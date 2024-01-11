import Grid from "@mui/material/Grid";
import styles from "./close-button.module.scss";
import Image from "next/image";
import { useAppDispatch } from "@/state-mgmt/store/hooks";
import { setPlayButtonClickSound } from "@/state-mgmt/app/appSlice";

interface Props {
  handleClose: Function;
  smallViewport?: boolean;
}

const CloseButton: React.FC<Props> = (props: Props) => {
  const { handleClose, smallViewport } = props;

  const dispatch = useAppDispatch();

  const handleCloseClick = () => {
    dispatch(setPlayButtonClickSound(true));
    handleClose();
  };

  return (
    <Image
      src="/img/unity-assets/shared/close.png"
      height={25}
      width={25}
      className={styles.clickableImage}
      onClick={() => handleCloseClick()}
    />
  );
};

export default CloseButton;
