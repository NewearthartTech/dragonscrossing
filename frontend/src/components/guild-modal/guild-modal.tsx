import Grid from "@mui/material/Grid";
import styles from "./guild-modal.module.scss";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import Modal from "@mui/material/Modal";
import DCXButton from "@/components/dcx-button/dcx-button";
import { useState } from "react";
import useWindowDimensions from "@/helpers/window-dimensions";
import { selectDisplayGuildModal, setDisplayGuildModal } from "@/state-mgmt/app/appSlice";
import Image from "next/image";
import { mdScreenWidth, xsScreenWidth } from "@/helpers/global-constants";
import Summoning from "../summoning/summoning";
import NftActions from "../nftActions/nftActions";
import Skills from "../skills/skills";
import CloseButton from "../close-button/close-button";
import Scrollbars from "react-custom-scrollbars";

interface Props {}

const GuildModal: React.FC<Props> = (props: Props) => {
  const { width, height } = useWindowDimensions();
  const dispatch = useAppDispatch();
  const displayGuildModal = useAppSelector(selectDisplayGuildModal);

  const [showShards, setShowShards] = useState(true);
  const [showActions, setShowActions] = useState(false);
  const [showUnidentified, setShowUnidentified] = useState(false);
  const [showIdentified, setShowIdentified] = useState(false);

  const handleCloseGuildModal = () => {
    dispatch(setDisplayGuildModal(false));
  };

  const handleGuildButtonClicked = (button: 'shards'|'unidentified'|'identified'|'actions') => {

    switch(button){
      case "shards":
        setShowUnidentified(false);
        setShowIdentified(false);
        setShowActions(false);
        setShowShards(true);
        break;
      
      case "unidentified":
        setShowIdentified(false);
        setShowShards(false);
        setShowActions(false);
        setShowUnidentified(true);
        break;

      case "identified":
        setShowShards(false);
        setShowUnidentified(false);
        setShowActions(false);
        setShowIdentified(true);
        break;

      case 'actions':
        setShowShards(false);
        setShowUnidentified(false);
        setShowActions(true);
        setShowIdentified(false);
        break;
    }

    if (button === "shards") {
    } else if (button === "unidentified") {
      
    } else {
      
    }
  };

  return (
    <Modal open={displayGuildModal} onClose={handleCloseGuildModal} className={styles.modalMain}>
      <Grid container className={showShards ? styles.summoningMainContainer : styles.skillsMainContainer}>
        <Grid
          container
          className={showShards ? styles.summoningCloseButtonContainer : styles.skillsCloseButtonContainer}
        >
          <CloseButton handleClose={handleCloseGuildModal} />
        </Grid>
        <Grid container className={styles.guildButtonsContainer} style={{ top: showShards ? 0 : 9 }}>
          
          <DCXButton
            title="UNIDENTIFED"
            height={34}
            width={110}
            color="blue"
            inactive={!showUnidentified}
            onClick={() => handleGuildButtonClicked("unidentified")}
          />
          <DCXButton
            title="IDENTIFIED"
            height={34}
            width={110}
            color="blue"
            inactive={!showIdentified}
            onClick={() => handleGuildButtonClicked("identified")}
          />
          <DCXButton
            title="SHARDS"
            height={34}
            width={110}
            color="blue"
            inactive={!showShards}
            onClick={() => handleGuildButtonClicked("shards")}
          />
          <DCXButton
            title="ACTIONS"
            height={34}
            width={110}
            color="blue"
            inactive={!showActions}
            onClick={() => handleGuildButtonClicked("actions")}
          />
        </Grid>
        <Grid container className={showShards ? styles.summoningContainer : styles.identifyContainer}>
          {(showUnidentified || showIdentified) && <Grid container className={styles.shadowContainer} />}
          {showShards && (
            <Grid container className={styles.summoningTabContainer}>
              <Summoning />
            </Grid>
          )}
          {showActions && (
            <Grid container className={styles.summoningTabContainer}>
              <NftActions />
            </Grid>
          )}
          {(showUnidentified || showIdentified) && (
            <Grid container className={styles.identifyTabContainer}>
              <Grid container className={styles.skillsBackgroundContainer}>
                <Image
                  src={
                    width > xsScreenWidth
                      ? "/img/unity-assets/shared/action_bg.png"
                      : "/img/unity-assets/shared/action_bg_vertical_small.png"
                  }
                  height={width > mdScreenWidth ? 515 : width > xsScreenWidth ? 360 : 335}
                  width={width > mdScreenWidth ? 705 : width > xsScreenWidth ? 487 : 300}
                  quality={100}
                />
              </Grid>
              <Scrollbars
                renderThumbVertical={() => (
                  <Grid
                    container
                    style={{
                      width: "5px",
                      borderRadius: "4px",
                      backgroundColor: "rgb(230, 230, 230)",
                    }}
                  />
                )}
              >
                <Skills page={showUnidentified ? "identify" : "learn"} />
              </Scrollbars>
            </Grid>
          )}
        </Grid>
      </Grid>
    </Modal>
  );
};

export default GuildModal;
