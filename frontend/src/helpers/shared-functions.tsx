import DCXTile from "@/components/dcx-tile/dcx-tile";
import DCXUndiscoveredTile from "@/components/dcx-undiscovered-tile/dcx-undiscovered-tile";
import { GameStateDto } from "@dcx/dcx-backend";

export const renderDiscoveredTiles = (gameState: GameStateDto): Array<JSX.Element> => {
  const tiles: JSX.Element[] = [];
  if (gameState.zone.discoveredTiles && gameState.zone.discoveredTiles.length > 0) {
    gameState.zone.discoveredTiles.forEach((tile) => {
      tiles.push(
        <DCXTile
          tileName={tile.slug}
          tileType={tile.tileType}
          disable={!tile.isActive}
          disableView={!tile.isActive}
          key={tile.slug}
        />
      );
    });
  }
  return tiles;
};

export const renderUndiscoveredTiles = (gameState: GameStateDto): Array<JSX.Element> => {
  const undiscoveredTiles: JSX.Element[] = [];
  if (gameState.zone.undiscoveredTileCount > 0) {
    for (let i = 0; i < gameState.zone.undiscoveredTileCount; i++) {
      undiscoveredTiles.push(<DCXUndiscoveredTile key={i} />);
    }
  }
  return undiscoveredTiles;
};

export const formatErrorMessage = (err: any): string => {
  let errorMessage = "";
  err.response.data
    ? (errorMessage = err.response.data.errorId + ": " + err.response.data.message)
    : (errorMessage = err.message);
  return errorMessage;
};

export const rng = (minNumber: number, maxNumber: number) => {
  return Math.floor(Math.random() * maxNumber) + minNumber;
};

export const unixToLocalDateTime = (unixTimestamp: string): string => {
  const date = new Date(unixTimestamp);

  const year = date.getUTCFullYear();
  const month = date.getUTCMonth() + 1; // 0 based so add 1
  const day = date.getUTCDate();
  const hours = date.getUTCHours();
  const minutes = date.getUTCMinutes();
  const seconds = date.getUTCSeconds();

  return month + "-" + day + "-" + year + " " + hours + ":" + minutes + ":" + seconds;
};
