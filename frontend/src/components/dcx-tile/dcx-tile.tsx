import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import styles from "./dcx-tile.module.scss";
import Image from "next/image";
import useWindowDimensions from "@/helpers/window-dimensions";
import { tileDescriptionsMap, xsScreenWidth } from "@/helpers/global-constants";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import {
  selectGameState,
  updateGameState,
} from "@/state-mgmt/game-state/gameStateSlice";
import { setPlayTileClickSound } from "@/state-mgmt/app/appSlice";
import { TileBorderType } from "@/state-mgmt/app/appTypes";
import { DcxTiles, TileType } from "@dcx/dcx-backend";
import { useEffect, useState } from "react";
import { setAppMessage } from "@/state-mgmt/app/appSlice";

interface Props {
  tileName: DcxTiles | string;
  tileType?: TileType;
  disable?: boolean;
  disableView?: boolean;
  borderType?: TileBorderType;
}

const DCXTile: React.FC<Props> = (props: Props) => {
  const { tileName, tileType, disable, disableView, borderType } = props;
  const dispatch = useAppDispatch();
  const { width, height } = useWindowDimensions();

  const gameState = useAppSelector(selectGameState);

  const [formattedTileName, setFormattedTileName] = useState("");

  useEffect(() => {
    switch (tileName) {
      case DcxTiles.HerbalistAedos:
        setFormattedTileName("HERBALIST");
        break;
      case DcxTiles.HerbalistMysteriousForest:
        setFormattedTileName("HERBALIST");
        break;
      case DcxTiles.HerbalistFoulWastes:
        setFormattedTileName("HERBALIST");
        break;
      case DcxTiles.HerbalistTreacherousPeaks:
        setFormattedTileName("HERBALIST");
        break;
      case DcxTiles.HerbalistDarkTower:
        setFormattedTileName("HERBALIST");
        break;
      case DcxTiles.CampMysteriousForest:
        setFormattedTileName("CAMP");
        break;
      case DcxTiles.CampTreacherousPeaks:
        setFormattedTileName("CAMP");
        break;
      case DcxTiles.AdventuringGuild:
        setFormattedTileName("ADVENTURING GUILD");
        break;
      case DcxTiles.AdventuringGuildFoulWastes:
        setFormattedTileName("ADVENTURING GUILD");
        break;
      case DcxTiles.AncientBattlefield:
        setFormattedTileName("ANCIENT BATTLEFIELD");
        break;
      case DcxTiles.DarkTower:
        setFormattedTileName("DARK TOWER");
        break;
      case DcxTiles.EnchantedFields:
        setFormattedTileName("ENCHANTED FIELDS");
        break;
      case DcxTiles.FoulWastes:
        setFormattedTileName("FOUL WASTES");
        break;
      case DcxTiles.GriffonsNest:
        setFormattedTileName("GRIFFONS NEST");
        break;
      case DcxTiles.LabyrinthianDungeon:
        setFormattedTileName("LABYRINTHIAN DUNGEON");
        break;
      case DcxTiles.LibraryOfTheArchmage:
        setFormattedTileName("LIBRARY OF THE ARCHMAGE");
        break;
      case DcxTiles.MountainFortress:
        setFormattedTileName("MOUNTAIN FORTRESS");
        break;
      case DcxTiles.MysteriousForest:
        setFormattedTileName("MYSTERIOUS FOREST");
        break;
      case DcxTiles.OdorousBog:
        setFormattedTileName("ODOROUS BOG");
        break;
      case DcxTiles.PilgrimsClearing:
        setFormattedTileName("PILGRIMS CLEARING");
        break;
      case DcxTiles.SharedStash:
        setFormattedTileName("SHARED STASH");
        break;
      case DcxTiles.SlaversRow:
        setFormattedTileName("SLAVERS ROW");
        break;
      case DcxTiles.SummonersSummit:
        setFormattedTileName("SUMMONERS SUMMIT");
        break;
      case DcxTiles.SylvanWoodlands:
        setFormattedTileName("SYLVAN WOODLANDS");
        break;
      case DcxTiles.TreacherousPeaks:
        setFormattedTileName("TREACHEROUS PEAKS");
        break;
      case DcxTiles.WildPrairie:
        setFormattedTileName("WILD PRAIRIE");
        break;

      case DcxTiles.WondrousThicket:
        setFormattedTileName("WONDROUS THICKET");
        break;
      case DcxTiles.FeyClearing:
        setFormattedTileName("FEY CLEARING");
        break;
      case DcxTiles.ShatteredStable:
        setFormattedTileName("SHATTERED STABLE");
        break;
      case DcxTiles.ForebodingDale:
        setFormattedTileName("FOREBODING DALE");
        break;
      case DcxTiles.FallenTemples:
        setFormattedTileName("FALLEN TEMPLES");
        break;
      case DcxTiles.PillaredRuins:
        setFormattedTileName("PILLARED RUINS");
        break;
      case DcxTiles.Acropolis:
        setFormattedTileName("ACROPOLIS");
        break;
      case DcxTiles.DestroyedPantheon:
        setFormattedTileName("DESTROYED PANTHEON");
        break;

      default:
        const tileNameStr = tileName.toString().toLocaleLowerCase();
        if(tileNameStr.includes("adventuringguild")){
          setFormattedTileName("ADVENTURING GUILD"); 
        }else if(tileNameStr.includes("herbalist")){
          setFormattedTileName("HERBALIST");
        }else if(tileNameStr.includes("camp")){
          setFormattedTileName("CAMP");
        }else if(tileNameStr.includes("blacksmith")){
          setFormattedTileName("BLACKSMITH");
        }else{
          setFormattedTileName(tileName);
        }
        
    }
  }, []);

  const handleTileClick = () => {
    dispatch(setPlayTileClickSound(true));
    if (
      tileName === "camp" ||
      tileName === "camp_mysteriousForest" ||
      tileName === "camp_treacherousPeaks"
    ) {
      handleCampTileClicked();
    } else {
      dispatch(updateGameState(tileName as DcxTiles));
    }
  };

  const handleCampTileClicked = () => {
    dispatch(
      setAppMessage({
        message: `WHEN YOU CAMP, UNSECURED NFT ITEMS ARE SENT TO THE BLOCKCHAIN, YOUR HERO IS HEALED, AND ALL REMAINING QUESTS ARE CONSUMED AND COUNTED AGAINST YOU.`,
        isClearToken: false,
        buttonTitle: `CONFIRM CAMP`,
        isCamp: true,
      })
    );
  };

  const getTileSlug = (tileName: string) => {
    if (tileName.toString().includes("herbalist")) {
      return "herbalist";
    }
    if (tileName.toString().includes("camp")) {
      return "camp";
    }
    if (tileName.toString().includes("adventuringGuild")) {
      return "adventuringGuild";
    }
    return tileName;
  };

  return (
    <Grid
      container
      className={disable ? styles.tileContainerDisabled : styles.tileContainer}
    >
      {tileName && tileName.toString() !== "" && (
        <Grid
          container
          className={styles.tile}
          onClick={() => (disable ? undefined : handleTileClick())}
        >
          {disableView && <Grid container className={styles.tileDisabled} />}
          <Grid container className={styles.tileImageContainer}>
            <Image
              src={`/img/backgrounds/${getTileSlug(tileName)}-exterior.jpg`}
              height={width <= xsScreenWidth ? "190" : "238"}
              width={width <= xsScreenWidth ? "297" : "371"}
              className={disable ? undefined : styles.clickableTile}
            />
          </Grid>
          <Grid container className={styles.tileBorderContainer}>
            <Image
              src={
                borderType
                  ? `/img/backgrounds/${borderType}-tile-border.png`
                  : `/img/backgrounds/armored-tile-border.png`
              }
              height={width <= xsScreenWidth ? "225" : "295"}
              width={width <= xsScreenWidth ? "316" : "412"}
              className={disable ? undefined : styles.clickableTile}
            />
          </Grid>
          <Grid
            container
            direction="column"
            className={styles.tileTitleDescriptionContainer}
          >
            <Typography
              component="span"
              variant="h4"
              className={styles.tileTitle}
              style={{ cursor: disable ? "default" : "pointer" }}
            >
              {formattedTileName}
            </Typography>
            <Typography
              component="span"
              variant="h5"
              className={styles.tileDescription}
              style={{ cursor: disable ? "default" : "pointer" }}
            >
              {`(${
                tileName === DcxTiles.WildPrairie &&
                (gameState.zone.slug === DcxTiles.Aedos ||
                  gameState.zone.slug === DcxTiles.WildPrairie)
                  ? "ZONE"
                  : tileDescriptionsMap[tileName]
              })`}
            </Typography>
            {disableView && (
              <Typography
                component="span"
                className={styles.tileStatus}
                style={{ cursor: disable ? "default" : "pointer" }}
              >
                {`${
                  tileType === TileType.Daily
                    ? "DAILY QUEST COMPLETED"
                    : tileType === TileType.Boss
                    ? "BOSS QUEST COMPLETED"
                    : undefined
                }`}
              </Typography>
            )}
          </Grid>
        </Grid>
      )}
    </Grid>
  );
};

export default DCXTile;
