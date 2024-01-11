import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import styles from "./skills.module.scss";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import { useEffect } from "react";
import { resetForgetSkillStatus, selectForgetSkillStatus, selectSelectedHero } from "@/state-mgmt/hero/heroSlice";
import { Status } from "@/state-mgmt/app/appTypes";
import { ItemSlotTypeDto, LearnedHeroSkill, SkillItem, UnlearnedHeroSkill } from "@dcx/dcx-backend";
import DCXUnidentifiedSkill from "../dcx-skill/dcx-unidentified-skill/dcx-unidentified-skill";
import DCXUnlearnedSkill from "../dcx-skill/dcx-unlearned-skill/dcx-unlearned-skill";
import DCXLearnedSkill from "../dcx-skill/dcx-learned-skill/dcx-learned-skill";
import { selectWalletItems } from "@/state-mgmt/player/playerSlice";
import { selectDisplayGuildModal } from "@/state-mgmt/app/appSlice";
import { _USE_DEFAULT_CHAIN, _USE_SHADOW_CHAIN } from "../web3/contractCalls";

interface Props {
  page: string;
}

const Skills: React.FC<Props> = (props: Props) => {
  const dispatch = useAppDispatch();
  const forgetSkillStatus = useAppSelector(selectForgetSkillStatus);
  const { hero } = useAppSelector(selectSelectedHero);
  const walletItems = useAppSelector(selectWalletItems);
  const displayGuildModal = useAppSelector(selectDisplayGuildModal);
  const { page } = props;

  useEffect(() => {
    if (forgetSkillStatus.status === Status.Loaded) {
      dispatch(resetForgetSkillStatus());
    }
    if (forgetSkillStatus.status === Status.Failed) {
      dispatch(resetForgetSkillStatus());
    }
  }, [forgetSkillStatus]);

  const renderUnidentifiedSkills = () => {
    const skills: JSX.Element[] = [];

    const unidentifiedItems = walletItems.filter((wi) => wi.slot === ItemSlotTypeDto.UnidentifiedSkill);

    const unidentifiedSkillItems = unidentifiedItems.map((ui) => {
      if (ui.type === "SkillItem") {
        return ui as SkillItem;
      }
    });

    unidentifiedSkillItems.forEach((usi) => {
      if (usi) {
        skills.push(
          <Grid item className={styles.unidentifiedSkillContainer} key={usi.id}>
            <DCXUnidentifiedSkill skillItem={usi} />
          </Grid>
        );
      }
    });
    return skills;
  };

  const renderUnlearnedSkills = () => {
    const skills: JSX.Element[] = [];
    const unlearnedItems = walletItems.filter((wi) => wi.slot === ItemSlotTypeDto.UnlearnedSkill);

    const unlearnedSkillItems = unlearnedItems.map((ui) => {
      if (ui.type === "SkillItem") {
        return ui as SkillItem;
      }
    });

    unlearnedSkillItems.forEach((usi) => {
      if (usi && usi.nftTokenId) {
        skills.push(
          <Grid item className={styles.skillContainer} key={usi.id}>
            <DCXUnlearnedSkill
              skill={usi.skill as UnlearnedHeroSkill}
              skillId={usi.id}
              nftTokenId={usi.nftTokenId}
              page={page}
              disableActions={displayGuildModal}
              chainId={usi.isDefaultChain?_USE_DEFAULT_CHAIN:_USE_SHADOW_CHAIN}
            />
          </Grid>
        );
      }
    });
    return skills;
  };

  const renderSkills = () => {
    const skills: JSX.Element[] = [];
    hero.skills.forEach((skill: LearnedHeroSkill) => {
      skills.push(
        <Grid item className={styles.skillContainer} key={skill.id}>
          <DCXLearnedSkill skill={skill as LearnedHeroSkill} page={page} />
        </Grid>
      );
    });
    return skills;
  };

  return (
    <Grid container className={styles.main}>
      {page === "identify" && (
        <Grid container className={styles.container}>
          {walletItems && walletItems.filter((wi) => wi.slot === ItemSlotTypeDto.UnidentifiedSkill).length > 0 ? (
            renderUnidentifiedSkills()
          ) : (
            <Grid container className={styles.noSkillsMessageContainer}>
              <Typography component="span" className={styles.noSkillsMessage}>
                YOU HAVE NO UNIDENTIFIED SKILLS.
              </Typography>
            </Grid>
          )}
        </Grid>
      )}
      {page === "learn" && (
        <Grid container className={styles.container}>
          {walletItems && walletItems.filter((wi) => wi.slot === ItemSlotTypeDto.UnlearnedSkill).length > 0 ? (
            renderUnlearnedSkills()
          ) : (
            <Grid container className={styles.noSkillsMessageContainer}>
              <Typography component="span" className={styles.noSkillsMessage}>
                YOU HAVE NO UNLEARNED SKILLS.
              </Typography>
            </Grid>
          )}
        </Grid>
      )}
      {page === "equip" && (
        <Grid container className={styles.container}>
          {hero.skills && hero.skills.length > 0 ? (
            renderSkills()
          ) : (
            <Grid container className={styles.noSkillsMessageContainer}>
              <Typography component="span" className={styles.noSkillsMessage}>
                YOU HAVE NO LEARNED SKILLS.
              </Typography>
            </Grid>
          )}
        </Grid>
      )}
    </Grid>
  );
};

export default Skills;
