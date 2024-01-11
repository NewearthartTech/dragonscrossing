import { HeroToken, Status } from "@/state-mgmt/app/appTypes";
import {
  clearSelectedHero,
  getHeroes,
  getSelectedHero,
  getTokenHero,
  selectGetHeroesStatus,
  selectHeroes,
  selectSelectedHero,
  selectSelectedHeroStatus,
  setSelectedHero,
} from "@/state-mgmt/hero/heroSlice";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import { isBrowser } from "core/utils";
import { useRouter } from "next/router";
import { useEffect, useState } from "react";
import DetailedError from "../detailed-error/detailedError";
import { initializedHero, noHeroesMessage } from "@/helpers/global-constants";
import {
  getNftItems,
  resetRetrievePlayerStatus,
  retrievePlayer,
  selectAuthenticationStatus,
  selectPlayer,
  selectRetrievePlayerStatus,
  setConnectedWalletAddress,
  setDcxBalance,
  setPlayerSettingsInLocalStorage,
} from "@/state-mgmt/player/playerSlice";
import { IAsyncResult, ShowError } from "./apiErrors";
import { useAuthentication } from "../auth";
import { useConnectCalls } from "../web3";
import { Configuration, DcxTiles } from "@dcx/dcx-backend";
import {
  getGameState,
  resetGameStateStatuses,
  selectGameState,
  selectGameStateStatus,
} from "@/state-mgmt/game-state/gameStateSlice";
import {
  selectAppInitialized,
  selectBrowserNavigationUsed,
  selectRefreshHeroNFTs,
  selectRefreshHeroNFTsDelayed,
  selectRefreshItemNFTs,
  selectRefreshItemNFTsDelayed,
  setAppInitialized,
  setBrowserNavigationUsed,
  setRefreshHeroNFTs,
  setRefreshHeroNFTsDelayed,
  setRefreshItemNFTs,
  setRefreshItemNFTsDelayed,
} from "@/state-mgmt/app/appSlice";
import jwt_decode from "jwt-decode";
import Grid from "@mui/material/Grid";
import { PlayerSettings } from "@/state-mgmt/player/playerTypes";
import { getOpenSeasons, selectOpenSeasons } from "@/state-mgmt/season/seasonSlice";
import { clearUpdateOwnerNftIds, selectUpdateOwnerNftIds } from "@/state-mgmt/vendor/vendorSlice";
import { _USE_DEFAULT_CHAIN } from "../web3/contractCalls";

interface Props {
  comp: any;
  api: string;
}

export const apiConfig: Configuration = new Configuration({
  baseOptions: {
    headers: {
      Authorization: "",
    },
  },
  basePath: "",
});

const Verification: React.FC<Props> = (props: Props) => {
  const router = useRouter();
  const dispatch = useAppDispatch();
  const { connect } = useConnectCalls();

  const player = useAppSelector(selectPlayer);
  const heroList = useAppSelector(selectHeroes);
  const getHeroesStatus = useAppSelector(selectGetHeroesStatus);
  const selectedHero = useAppSelector(selectSelectedHero);
  const isAuthenticated = useAppSelector(selectAuthenticationStatus);
  const gameStateStatus = useAppSelector(selectGameStateStatus);
  const gameState = useAppSelector(selectGameState);
  const selectedHeroStatus = useAppSelector(selectSelectedHeroStatus);
  const browserNavigationUsed = useAppSelector(selectBrowserNavigationUsed);
  const isAppInitialized = useAppSelector(selectAppInitialized);
  const retrievePlayerStatus = useAppSelector(selectRetrievePlayerStatus);
  const openSeasons = useAppSelector(selectOpenSeasons);
  const refreshItemNFTs = useAppSelector(selectRefreshItemNFTs);
  const refreshItemNFTsDelayed = useAppSelector(selectRefreshItemNFTsDelayed);
  const refreshHeroNFTs = useAppSelector(selectRefreshHeroNFTs);
  const refreshHeroNFTsDelayed = useAppSelector(selectRefreshHeroNFTsDelayed);
  const updateOwnerNftIds = useAppSelector(selectUpdateOwnerNftIds);

  const [isGetHeroesResponse, setGetHeroesResponse] = useState(false);

  const { ensureLogin, authenticatedUser } = useAuthentication();

  const [authState, setAuthState] = useState<IAsyncResult<boolean>>();

  useEffect(() => {
    apiConfig.basePath = props.api;
    if (openSeasons.length === 0) {
      dispatch(getOpenSeasons());
    }
    if (!isAppInitialized) {
      const playerSettings: PlayerSettings = JSON.parse(localStorage.getItem("playerSettings") || "{}");
      if (playerSettings && Object.keys(playerSettings).length > 0) {
        dispatch(setPlayerSettingsInLocalStorage(playerSettings));
      }
      dispatch(setAppInitialized());
    }
  }, []);

  useEffect(() => {
    // Will run when leaving the current page; on back/forward actions
    router.beforePopState(({ as }) => {
      if (as !== router.asPath) {
        dispatch(setBrowserNavigationUsed(true));
      }
      return true;
    });
    return () => {
      router.beforePopState(() => true);
    };
  }, [router]);

  useEffect(() => {
    if (browserNavigationUsed) {
      dispatch(setSelectedHero(initializedHero));
      router.reload();
    }
  }, [browserNavigationUsed]);

  useEffect(() => {
    if (selectedHeroStatus.status === Status.Loaded && gameState.zone.slug === DcxTiles.Unknown) {
      dispatch(getGameState(selectedHero.hero.id));
    }
  }, [selectedHeroStatus]);

  useEffect(() => {
    if (gameStateStatus.status === Status.Loaded) {
      // dispatch an action to update the hero when gameState updates
      dispatch(getSelectedHero());
      router.push(gameState.slug);
      dispatch(resetGameStateStatuses());
    }
  }, [gameStateStatus]);

  useEffect(() => {
    // TODO: Do we need to reload the app if this call fails? See if this needs to be uncommented.
    // if (getHeroesStatus.status === Status.Failed) {
    //   localStorage.removeItem("login_key");
    //   router.reload();
    // }
    if (getHeroesStatus.status === Status.Loaded) {
      if (authenticatedUser && authenticatedUser.jwt) {
        const decodedToken: HeroToken = jwt_decode(authenticatedUser.jwt);
        if (decodedToken.ClaimWalletVarified && decodedToken.ClaimWalletVarified !== "") {
          dispatch(retrievePlayer());
          dispatch(setConnectedWalletAddress(decodedToken.ClaimWalletVarified));
        }
        if (decodedToken.SelectedHeroId && decodedToken.SelectedHeroId !== "" && authenticatedUser.seasonId) {
          dispatch(getTokenHero());
        }
      }
    }
  }, [getHeroesStatus]);

  useEffect(() => {
    (async () => {
      try {
        const auth = await ensureLogin("wallet");
      } catch (error: any) {
        // If ensureLogin fails, clear local storage and hard reload
        setAuthState({ error });
        localStorage.removeItem("login_key");
        router.reload();
      }
    })();
  }, [authenticatedUser]);

  useEffect(() => {
    (async () => {
      if (getHeroesStatus.status === Status.Loaded) {
        if (authenticatedUser && authenticatedUser.jwt) {
          const decodedToken: HeroToken = jwt_decode(authenticatedUser.jwt);
          if (!decodedToken.SelectedHeroId || decodedToken.SelectedHeroId === "") {
            console.debug("token does not contain a valid heroId, redirecting to heroSelect");
            dispatch(clearSelectedHero());
            router.push("/heroSelect");
          }
        }
      }
    })();
  }, [authenticatedUser]);

  useEffect(() => {
    (async () => {
      if (authenticatedUser && authenticatedUser.jwt) {
        apiConfig.baseOptions.headers.Authorization = "Bearer " + authenticatedUser.jwt;
        if (getHeroesStatus.status === Status.NotStarted) {
          if (!authenticatedUser.player?.blockchainPublicAddress) {
            console.warn("blockchainPublicAddress is empty");
            return;
          }

          getDcxBalance();

          const ownedHeroTokenIds = await (await connect(undefined)).getNfts("heroes");
          
          dispatch(getHeroes(ownedHeroTokenIds));
        } else {
          setGetHeroesResponse(true);
        }
      }
    })();
  }, [getHeroesStatus.status, authenticatedUser]);

  useEffect(() => {
    (async () => {
      if (authenticatedUser && authenticatedUser.jwt) {
        if (getHeroesStatus.status === Status.Loaded && refreshItemNFTs) {
          if (!authenticatedUser.player?.blockchainPublicAddress) {
            console.warn("blockchainPublicAddress is empty");
            return;
          }
          const ownedItemTokenIds = await (await connect(undefined)).getNfts("items", updateOwnerNftIds);

          if (ownedItemTokenIds.length > 0) {
            dispatch(getNftItems(ownedItemTokenIds));
          }
          dispatch(setRefreshItemNFTs(false));
          dispatch(clearUpdateOwnerNftIds());
        }
      }
    })();
  }, [getHeroesStatus, authenticatedUser, refreshItemNFTs]);

  useEffect(() => {
    if (refreshItemNFTsDelayed) {
      const timer = setTimeout(() => {
        dispatch(setRefreshItemNFTs(true));
        dispatch(setRefreshItemNFTsDelayed(false));
      }, 20000);
    }
  }, [refreshItemNFTsDelayed]);

  useEffect(() => {
    (async () => {
      if (authenticatedUser && authenticatedUser.jwt) {
        if (refreshHeroNFTs) {
          if (!authenticatedUser.player?.blockchainPublicAddress) {
            console.warn("blockchainPublicAddress is empty");
            return;
          }
          
          const ownedHeroTokenIds = await (await connect(undefined)).getNfts("heroes", updateOwnerNftIds);
          
          if (ownedHeroTokenIds.length > 0) {
            dispatch(getHeroes(ownedHeroTokenIds));
          }
          dispatch(setRefreshHeroNFTs(false));
          dispatch(clearUpdateOwnerNftIds());
        }
      }
    })();
  }, [authenticatedUser, refreshHeroNFTs]);

  useEffect(() => {
    if (refreshHeroNFTsDelayed) {
      const timer2 = setTimeout(() => {
        dispatch(setRefreshHeroNFTs(true));
        dispatch(setRefreshHeroNFTsDelayed(false));
      }, 20000);
    }
  }, [refreshHeroNFTsDelayed]);

  useEffect(() => {
    if (retrievePlayerStatus.status === Status.Loaded) {
      dispatch(resetRetrievePlayerStatus());
    }
    if (retrievePlayerStatus.status === Status.Failed) {
      dispatch(resetRetrievePlayerStatus());
    }
  }, [retrievePlayerStatus]);

  useEffect(() => {
    if (selectedHero.hero && !selectedHero.hero.id && router.pathname === "/") {
      dispatch(clearSelectedHero());
    }
  }, [router]);

  const getDcxBalance = async () => {
    const dcxBalance = await (await connect(undefined)).getDcxBalance();
    dispatch(setDcxBalance(+Number(dcxBalance).toFixed(2)));
  };

  if (!authenticatedUser) {
    return (
      <Grid
        container
        style={{
          height: "100vh",
          width: "100vw",
          justifyContent: "center",
          alignItems: "center",
        }}
      >
        {authState?.error && <ShowError error={authState.error} />}
      </Grid>
    );
  }

  const selectedHeroCheck = () => {
    if (
      (router.pathname === "/heroSelect" && (!selectedHero.hero || selectedHero.hero.id === -1)) ||
      (router.pathname !== "/heroSelect" && selectedHero.hero.id !== -1)
    ) {
      return true;
    } else {
      return false;
    }
  };

  if (isBrowser()) {
    if (isAuthenticated) {
      if (getHeroesStatus.status === Status.Loaded && heroList.heroes.length === 0) {
        return <DetailedError message={noHeroesMessage} />;
      }
      if (router.pathname !== "/heroSelect" && (!selectedHero || selectedHero.hero.id === -1)) {
        if (authenticatedUser && authenticatedUser.jwt) {
          const decodedToken: HeroToken = jwt_decode(authenticatedUser.jwt);
          if (!decodedToken.SelectedHeroId || decodedToken.SelectedHeroId === "") {
            router.push("/heroSelect");
          }
        }
      }
    } else {
      if (router.pathname !== "/") {
        router.push("/");
      }
    }
  }

  return (
    <>
      {isBrowser() &&
        isAuthenticated &&
        authenticatedUser &&
        isGetHeroesResponse &&
        heroList.heroes.length > 0 &&
        selectedHeroCheck() && <props.comp />}
    </>
  );
};

export default Verification;
