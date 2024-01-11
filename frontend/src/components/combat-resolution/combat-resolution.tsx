import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import Modal from "@mui/material/Modal";
import styles from "./combat-resolution.module.scss";
import { useEffect, useState } from "react";
import DCXButton from "@/components/dcx-button/dcx-button";
import Image from "next/image";
import { useRouter } from "next/router";

interface Props {
  isVictory: boolean;
}

const CombatResolution: React.FC<Props> = (props: Props) => {
  const router = useRouter();

  const { isVictory } = props;

  const [displayModal, setDisplayModal] = useState(true);

  useEffect(() => {}, []);

  const handleButtonClick = () => {
    if (isVictory) {
      setDisplayModal(false);
      router.push("/plunder");
    } else {
      setDisplayModal(false);
      router.push("/death");
    }
  };

  return (
    <Grid container direction="row">
      <Modal open={displayModal} className={styles.modalMain}>
        <Grid container direction="row" className={styles.modalContainer}>
          <Grid container className={styles.container}>
            <Grid container className={styles.backgroundImage}>
              <Image
                src="/img/unity-assets/shared/action_bg.png"
                height={300}
                width={390}
                quality={100}
              />
            </Grid>
            <Grid item className={styles.topLeft}>
              <Image
                src="/img/unity-assets/shared/window_top_left.png"
                height={50}
                width={70}
                quality={100}
              />
            </Grid>
            <Grid item className={styles.topRight}>
              <Image
                src="/img/unity-assets/shared/window_top_right.png"
                height={50}
                width={70}
                quality={100}
              />
            </Grid>
            <Grid item className={styles.bottomLeft}>
              <Image
                src="/img/unity-assets/shared/window_bottom_left.png"
                height={30}
                width={43}
                quality={100}
              />
            </Grid>
            <Grid item className={styles.bottomRight}>
              <Image
                src="/img/unity-assets/shared/window_bottom_right.png"
                height={30}
                width={43}
                quality={100}
              />
            </Grid>
            <Grid container className={styles.victoryContainer}>
              <Image
                src="/img/unity-assets/combat/action_result_bg.png"
                height={140}
                width={137}
                quality={100}
              />
              <Typography
                component="span"
                className={isVictory ? styles.victoryText : styles.defeatText}
              >
                {isVictory ? "Victory" : "Defeat"}
              </Typography>
            </Grid>
            <Grid container className={styles.button}>
              <DCXButton
                title={isVictory ? "VIEW LOOT" : "CONTINUE"}
                height={42}
                width={144}
                color={isVictory ? "blue" : "red"}
                onClick={handleButtonClick}
              />
            </Grid>
          </Grid>
        </Grid>
      </Modal>
    </Grid>
  );
};

export default CombatResolution;
