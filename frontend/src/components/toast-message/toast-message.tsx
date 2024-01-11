import Grid from "@mui/material/Grid";
import styles from "./toast-message.module.scss";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import {
  selectSnackbarMessage,
  setSnackbarMessage,
} from "@/state-mgmt/app/appSlice";
import Snackbar from "@mui/material/Snackbar";
import SnackbarContent from "@mui/material/SnackbarContent";
import IconButton from "@mui/material/IconButton";
import CloseIcon from "@mui/icons-material/Close";

interface Props {}

const ToastMessage: React.FC<Props> = (props: Props) => {
  const dispatch = useAppDispatch();
  const snackbarMessage = useAppSelector(selectSnackbarMessage);

  const handleClose = () => {
    dispatch(setSnackbarMessage({ isOpen: false, message: "" }));
  };

  return (
    <Grid container className={styles.container}>
      <Snackbar
        anchorOrigin={{ vertical: "top", horizontal: "right" }}
        autoHideDuration={5000}
        open={snackbarMessage.isOpen}
        onClose={handleClose}
      >
        <SnackbarContent
          message={snackbarMessage.message}
          className={styles.messageText}
          action={
            <IconButton key="close" onClick={handleClose}>
              <CloseIcon style={{ color: "white" }} />
            </IconButton>
          }
        />
      </Snackbar>
    </Grid>
  );
};

export default ToastMessage;
