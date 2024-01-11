import LinkWallet from "@/components/link-wallet/link-wallet";
import { Hero } from "@/state-mgmt/hero/heroTypes";
import { selectAuthenticationStatus, selectPlayer, validateCredentials } from "@/state-mgmt/player/playerSlice";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import Grid from "@mui/material/Grid";
import Button from "@mui/material/Button";
import TextField from "@mui/material/TextField";
import { isBrowser } from "core/utils";
import type { NextPage } from "next";
import { useRouter } from "next/router";
import { useEffect, useState } from "react";
import styles from "./landing-page.module.scss";

interface Props {}

const LandingPage: NextPage<Props> = (props: Props) => {
  const router = useRouter();
  const dispatch = useAppDispatch();
  const isAuthenticated = useAppSelector(selectAuthenticationStatus);
  const player = useAppSelector(selectPlayer);

  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");

  useEffect(() => {
    if (isAuthenticated) {
      router.push("/heroSelect");
    }
  }, [isAuthenticated]);

  const handleUsernameChange = (e: any) => {
    setUsername(e.target.value);
  };

  const handlePasswordChange = (e: any) => {
    setPassword(e.target.value);
  };

  const validate = () => {
    dispatch(validateCredentials({ username: username, password: password }));
  };

  if (player.blockchainPublicAddress !== "") {
    return (
      <Grid container direction="column" className={styles.main}>
        <Grid container direction="column" className={styles.container}>
          <TextField
            label="Username"
            variant="standard"
            className={styles.input}
            onChange={handleUsernameChange}
            value={username}
          />
          <TextField
            label="Password"
            variant="standard"
            type="password"
            className={styles.input}
            onChange={handlePasswordChange}
            value={password}
          />
          <Button size="small" variant="contained" onClick={validate} className={styles.loginButton}>
            Login
          </Button>
        </Grid>
      </Grid>
    );
  } else {
    if (isBrowser()) {
      return <LinkWallet />;
    } else {
      return null;
    }
  }
};

export default LandingPage;
