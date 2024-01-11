import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import styles from "./dcx-button.module.scss";
import Image from "next/image";
import { useAppDispatch } from "@/state-mgmt/store/hooks";
import { setPlayButtonClickSound } from "@/state-mgmt/app/appSlice";

interface Props {
  title: string;
  height: number;
  width: number;
  color: string;
  onClick: Function;
  hideArrows?: boolean;
  disabled?: boolean;
  inactive?: boolean;
  fontSize?: number;
  marginTop?: number;
  marginBottom?: number;
  marginLeft?: number;
  marginRight?: number;
  arrowTopAdjustment?: number;
  disabledLayerHeightAdjustment?: number;
  disabledLayerWidthAdjustment?: number;
}

const DCXButton: React.FC<Props> = (props: Props) => {
  const {
    title,
    height,
    width,
    color,
    onClick,
    hideArrows,
    disabled,
    inactive,
    fontSize,
    marginTop,
    marginBottom,
    marginLeft,
    marginRight,
    arrowTopAdjustment,
    disabledLayerHeightAdjustment,
    disabledLayerWidthAdjustment,
  } = props;

  const dispatch = useAppDispatch();

  const handleOnClick = () => {
    dispatch(setPlayButtonClickSound(true));
    onClick();
  };

  return (
    <Grid
      container
      onClick={() => !disabled && handleOnClick()}
      className={styles.actionButtonContainer}
      style={{
        height: height,
        width: width + 2,
        minWidth: width + 2,
        marginTop: marginTop,
        marginBottom: marginBottom,
        marginLeft: marginLeft,
        marginRight: marginRight,
        cursor: disabled ? "" : "pointer",
      }}
    >
      <Image
        src="/img/unity-assets/shared/button/button_border.png"
        className={styles.actionButton}
        height={height}
        width={width}
        quality={100}
      />
      {!hideArrows && (
        <Grid
          container
          className={styles.borderLeftArrow}
          style={{ top: arrowTopAdjustment }}
        >
          <Image
            src="/img/unity-assets/shared/button/border_left_arrow.png"
            height={14}
            width={12}
            quality={100}
          />
        </Grid>
      )}
      {!hideArrows && (
        <Grid
          container
          className={styles.borderRightArrow}
          style={{ left: width - 10, top: arrowTopAdjustment }}
        >
          <Image
            src="/img/unity-assets/shared/button/border_right_arrow.png"
            height={14}
            width={12}
            quality={100}
          />
        </Grid>
      )}
      <Grid container className={styles.actionButton}>
        <Image
          src={`/img/unity-assets/shared/button/${color}_button.png`}
          height={height}
          width={width}
          quality={100}
        />
      </Grid>
      <Typography
        component="span"
        variant="body2"
        className={styles.actionButtonText}
        style={{ fontSize: fontSize }}
      >
        {title}
      </Typography>
      <Grid
        container
        className={
          disabled
            ? styles.disableButton
            : inactive
            ? styles.inactiveButton
            : styles.hoverLayer
        }
        style={{
          height:
            height -
            (disabledLayerHeightAdjustment ? disabledLayerHeightAdjustment : 8),
          width:
            width -
            (disabledLayerWidthAdjustment ? disabledLayerWidthAdjustment : 16),
        }}
      />
    </Grid>
  );
};

export default DCXButton;
