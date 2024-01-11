import { ItemStatType } from "@/state-mgmt/item/itemTypes";
import { DieDto, HeroDto, ItemDto, Range } from "@dcx/dcx-backend";

export const handleRightClick = (event: any) => {
  event.preventDefault();
};

export const calculateTotalChanceToDodge = (hero: HeroDto) => {
  // convert difficultyToHit and chanceToDodge to decimal value (250 = .25)
  const difficultyToHit = hero.calculatedStats.difficultyToHit / 10000;
  const chanceToDodge = hero.calculatedStats.chanceToDodge / 10000;

  const complementaryEvent = 1 - difficultyToHit;
  const totalChanceToDodge = (chanceToDodge * complementaryEvent + difficultyToHit) * 100;
  // Round to 2 decimal places when showing the percent
  return Math.round(totalChanceToDodge * 100) / 100;
};

export const calculateDamageRange = (dice: Array<DieDto>, bonusDamage: number): string => {
  let min = dice.length;
  let max = 0;
  dice.forEach((d) => {
    max += d.sides;
  });
  min += bonusDamage;
  max += bonusDamage;
  return `${min}-${max}`;
};

export const compareItemStatByType = (
  statType: ItemStatType,
  item: ItemDto,
  equippedItem: ItemDto,
  secondEquippedItem?: ItemDto
): number => {
  let differential = 0;
  switch (statType) {
    case ItemStatType.Agility:
      if (item.affectedAttributes.agility) {
        differential = item.affectedAttributes.agility;
        if (equippedItem.affectedAttributes.agility) {
          differential = differential - equippedItem.affectedAttributes.agility;
        }
        if (secondEquippedItem && secondEquippedItem.affectedAttributes.agility) {
          differential = differential - secondEquippedItem.affectedAttributes.agility;
        }
        // if (item.affectedAttributes.agility === differential) {
        //   differential = 0;
        // }
      }
      break;
    case ItemStatType.ArmorMitigation:
      if (item.affectedAttributes.armorMitigation) {
        differential = item.affectedAttributes.armorMitigation;
        if (equippedItem.affectedAttributes.armorMitigation) {
          differential = differential - equippedItem.affectedAttributes.armorMitigation;
        }
        if (secondEquippedItem && secondEquippedItem.affectedAttributes.armorMitigation) {
          differential = differential - secondEquippedItem.affectedAttributes.armorMitigation;
        }
        // if (item.affectedAttributes.armorMitigation === differential) {
        //   differential = 0;
        // }
        differential = differential / 100;
      }
      break;
    case ItemStatType.ArmorMitigationAmount:
      if (item.affectedAttributes.armorMitigationAmount) {
        differential = item.affectedAttributes.armorMitigationAmount;
        if (equippedItem.affectedAttributes.armorMitigationAmount) {
          differential = differential - equippedItem.affectedAttributes.armorMitigationAmount;
        }
        if (secondEquippedItem && secondEquippedItem.affectedAttributes.armorMitigationAmount) {
          differential = differential - secondEquippedItem.affectedAttributes.armorMitigationAmount;
        }
        // if (item.affectedAttributes.armorMitigationAmount === differential) {
        //   differential = 0;
        // }
      }
      break;
    case ItemStatType.BonusDamage:
      if (item.affectedAttributes.bonusDamage) {
        differential = item.affectedAttributes.bonusDamage;
        if (equippedItem.affectedAttributes.bonusDamage) {
          differential = differential - equippedItem.affectedAttributes.bonusDamage;
        }
        if (secondEquippedItem && secondEquippedItem.affectedAttributes.bonusDamage) {
          differential = differential - secondEquippedItem.affectedAttributes.bonusDamage;
        }
        // if (item.affectedAttributes.bonusDamage === differential) {
        //   differential = 0;
        // }
      }
      break;
    case ItemStatType.ChanceToHit:
      if (item.affectedAttributes.chanceToHit) {
        differential = item.affectedAttributes.chanceToHit;
        if (equippedItem.affectedAttributes.chanceToHit) {
          differential = differential - equippedItem.affectedAttributes.chanceToHit;
        }
        if (secondEquippedItem && secondEquippedItem.affectedAttributes.chanceToHit) {
          differential = differential - secondEquippedItem.affectedAttributes.chanceToHit;
        }
        // if (item.affectedAttributes.chanceToHit === differential) {
        //   differential = 0;
        // }
        differential = differential / 100;
      }
      break;
    case ItemStatType.Charisma:
      if (item.affectedAttributes.charisma) {
        differential = item.affectedAttributes.charisma;
        if (equippedItem.affectedAttributes.charisma) {
          differential = differential - equippedItem.affectedAttributes.charisma;
        }
        if (secondEquippedItem && secondEquippedItem.affectedAttributes.charisma) {
          differential = differential - secondEquippedItem.affectedAttributes.charisma;
        }
        // if (item.affectedAttributes.charisma === differential) {
        //   differential = 0;
        // }
      }
      break;
    case ItemStatType.CriticalHitRate:
      if (item.affectedAttributes.criticalHitRate) {
        differential = item.affectedAttributes.criticalHitRate;
        if (equippedItem.affectedAttributes.criticalHitRate) {
          differential = differential - equippedItem.affectedAttributes.criticalHitRate;
        }
        if (secondEquippedItem && secondEquippedItem.affectedAttributes.criticalHitRate) {
          differential = differential - secondEquippedItem.affectedAttributes.criticalHitRate;
        }
        // if (item.affectedAttributes.criticalHitRate === differential) {
        //   differential = 0;
        // }
        differential = differential / 100;
      }
      break;
    case ItemStatType.DifficultyToHit:
      if (item.affectedAttributes.difficultyToHit) {
        differential = item.affectedAttributes.difficultyToHit;
        if (equippedItem.affectedAttributes.difficultyToHit) {
          differential = differential - equippedItem.affectedAttributes.difficultyToHit;
        }
        if (secondEquippedItem && secondEquippedItem.affectedAttributes.difficultyToHit) {
          differential = differential - secondEquippedItem.affectedAttributes.difficultyToHit;
        }
        // if (item.affectedAttributes.difficultyToHit === differential) {
        //   differential = 0;
        // }
        differential = differential / 100;
      }
      break;
    case ItemStatType.DodgeRate:
      if (item.affectedAttributes.dodgeRate) {
        differential = item.affectedAttributes.dodgeRate;
        if (equippedItem.affectedAttributes.dodgeRate) {
          differential = differential - equippedItem.affectedAttributes.dodgeRate;
        }
        if (secondEquippedItem && secondEquippedItem.affectedAttributes.dodgeRate) {
          differential = differential - secondEquippedItem.affectedAttributes.dodgeRate;
        }
        // if (item.affectedAttributes.dodgeRate === differential) {
        //   differential = 0;
        // }
        differential = differential / 100;
      }
      break;
    case ItemStatType.HitPoints:
      if (item.affectedAttributes.hitPoints) {
        differential = item.affectedAttributes.hitPoints;
        if (equippedItem.affectedAttributes.hitPoints) {
          differential = differential - equippedItem.affectedAttributes.hitPoints;
        }
        if (secondEquippedItem && secondEquippedItem.affectedAttributes.hitPoints) {
          differential = differential - secondEquippedItem.affectedAttributes.hitPoints;
        }
        // if (item.affectedAttributes.hitPoints === differential) {
        //   differential = 0;
        // }
      }
      break;
    case ItemStatType.ParryRate:
      if (item.affectedAttributes.parryRate) {
        differential = item.affectedAttributes.parryRate;
        if (equippedItem.affectedAttributes.parryRate) {
          differential = differential - equippedItem.affectedAttributes.parryRate;
        }
        if (secondEquippedItem && secondEquippedItem.affectedAttributes.parryRate) {
          differential = differential - secondEquippedItem.affectedAttributes.parryRate;
        }
        // if (item.affectedAttributes.parryRate === differential) {
        //   differential = 0;
        // }
        differential = differential / 100;
      }
      break;
    case ItemStatType.Quickness:
      if (item.affectedAttributes.quickness) {
        differential = item.affectedAttributes.quickness;
        if (equippedItem.affectedAttributes.quickness) {
          differential = differential - equippedItem.affectedAttributes.quickness;
        }
        if (secondEquippedItem && secondEquippedItem.affectedAttributes.quickness) {
          differential = differential - secondEquippedItem.affectedAttributes.quickness;
        }
        // if (item.affectedAttributes.quickness === differential) {
        //   differential = 0;
        // }
      }
      break;
    case ItemStatType.Strength:
      if (item.affectedAttributes.strength) {
        differential = item.affectedAttributes.strength;
        if (equippedItem.affectedAttributes.strength) {
          differential = differential - equippedItem.affectedAttributes.strength;
        }
        if (secondEquippedItem && secondEquippedItem.affectedAttributes.strength) {
          differential = differential - secondEquippedItem.affectedAttributes.strength;
        }
        // if (item.affectedAttributes.strength === differential) {
        //   differential = 0;
        // }
      }
      break;
    case ItemStatType.Wisdom:
      if (item.affectedAttributes.wisdom) {
        differential = item.affectedAttributes.wisdom;
        if (equippedItem.affectedAttributes.wisdom) {
          differential = differential - equippedItem.affectedAttributes.wisdom;
        }
        if (secondEquippedItem && secondEquippedItem.affectedAttributes.wisdom) {
          differential = differential - secondEquippedItem.affectedAttributes.wisdom;
        }
        // if (item.affectedAttributes.wisdom === differential) {
        //   differential = 0;
        // }
      }
      break;
    default:
      null;
  }

  return differential;
};
