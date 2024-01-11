import React from "react";
import { AuthenticatedUser, AuthApi } from "@dcx/dcx-backend";
import { useConnectCalls } from "../web3";
import styles from "./authDlg.module.scss";
import Grid from "@mui/material/Grid";
import Modal from "@mui/material/Modal";
import Image from "next/image";
import DCXButton from "../dcx-button/dcx-button";
import Typography from "@mui/material/Typography";




export function AuthDlg({
  currentUser,
  onSignIn,
  onCancel,
}: {
  currentUser?: AuthenticatedUser;
  onSignIn: (details: AuthenticatedUser, authenticated: boolean) => void;
  onCancel?: (error: any) => void;
}) {
  const { connect, injectedAvailable } = useConnectCalls();

  const connectWallet = async () => {
    try {

      const api = new AuthApi(undefined, process.env.NEXT_PUBLIC_API || ".");
      const { data: authOptions } = await api.apiAuthGet();

      if (!authOptions?.web3Nounce) {
        throw new Error("web3 authentication is required");
      }

      const connected = await connect(undefined);

      const web3Signature = await connected.signMsg(authOptions.web3Nounce);

      console.log(
        `sign : ${web3Signature} , nounce = ${authOptions.web3Nounce}  `
      );

      
      const { data: signedInUser } = await api.apiAuthPost({
        options: {
          contractsForChain:{},
          web3Nounce:authOptions.web3Nounce
        },
        web3Signature,
        walletAddress: await connected.getAddress(),
      });

      if (!signedInUser.player?.id) throw new Error(`no user returned`);

      onSignIn(signedInUser, true);
    } catch (error: any) {
      onCancel && onCancel(error);
    }
  };

  return (
    <Modal
      open={true}
      onClose={() => {
        onCancel && onCancel(new Error("cancelled"));
      }}
      className={styles.modalMain}
    >
      <Grid container className={styles.container}>
        <Image
          src="/img/unity-assets/shared/action_bg.png"
          height={184}
          width={390}
          quality={100}
        />
        <Grid container className={styles.titleContainer}>
          <Typography
            component="span"
            className={styles.titleText}
          >{`DRAGON'S CROSSING`}</Typography>
        </Grid>
        <Grid container className={styles.content}>
          <Typography component="span" className={styles.contentText}>
            {injectedAvailable ? "...WAITING" : "PLEASE INSTALL METAMASK"}
          </Typography>
        </Grid>
        <Grid container className={styles.buttonContainer}>
          <DCXButton
            title="SIGN IN WITH METAMASK"
            height={42}
            width={160}
            color="red"
            disabled={!injectedAvailable}
            disabledLayerHeightAdjustment={11}
            disabledLayerWidthAdjustment={20}
            onClick={connectWallet}
          />
        </Grid>
      </Grid>
    </Modal>
  );
}
