import Grid from "@mui/material/Grid";
import Modal from "@mui/material/Modal";
import styles from "./app-message.module.scss";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import { selectAppMessage, setAppMessage, setDisplayCampModal } from "@/state-mgmt/app/appSlice";
import Image from "next/image";
import Typography from "@mui/material/Typography";
import { useRouter } from "next/router";
import CloseButton from "../close-button/close-button";
import DCXButton from "../dcx-button/dcx-button";
import { getCampOrders } from "@/state-mgmt/camp/campSlice";

interface Props {}

const AppMessage: React.FC<Props> = (props: Props) => {
  const router = useRouter();
  const dispatch = useAppDispatch();
  const appMessage = useAppSelector(selectAppMessage);

  const handleOnClick = () => {
    if (appMessage.isCamp) {
      goCamp();
    }
    handleClose();
  };

  const goCamp = () => {
    dispatch(setDisplayCampModal(true));
    dispatch(getCampOrders());
  };

  const handleClose = () => {
    dispatch(setAppMessage({ message: "", isClearToken: false, buttonTitle: "", isCamp: false }));
    if (appMessage.isClearToken) {
      localStorage.removeItem("login_key");
      router.reload();
    }
  };

  return (
    <Modal open={appMessage.message !== ""} onClose={handleClose} className={styles.modalMain}>
      <Grid container className={styles.container}>
        <Image src="/img/unity-assets/shared/tooltip_bg.png" height={150} width={280} quality={100} />
        <Grid container className={styles.closeButtonContainer}>
          <CloseButton handleClose={handleClose} />
        </Grid>
        <Grid container className={styles.messageContentContainer}>
          <Grid
            container
            direction="column"
            className={appMessage.isClearToken ? styles.messageContainerWithButton : styles.messageContainer}
          >
            <Typography component="p" className={styles.message}>
              {appMessage.isClearToken ? "INVALID TOKEN!" : appMessage.message}
            </Typography>
          </Grid>
          {appMessage.isClearToken && (
            <Grid container className={styles.buttonContainer}>
              <DCXButton title="REFRESH TOKEN" height={42} width={144} color="red" onClick={handleClose} />
            </Grid>
          )}
          {appMessage.buttonTitle && appMessage.buttonTitle !== "" && (
            <Grid container className={styles.buttonContainer}>
              <DCXButton
                title={appMessage.buttonTitle}
                height={42}
                width={144}
                color={appMessage.isCamp ? "red" : "blue"}
                onClick={handleOnClick}
              />
            </Grid>
          )}
        </Grid>
      </Grid>
    </Modal>
  );
};

export default AppMessage;
