import { Configuration, HeroMintOrderReq, HeroMintPriceReq, MintsApi } from "@dcx/dcx-backend";

export class MintApi {
  public async getMintPrice(heroMintPriceRequest: HeroMintPriceReq) {
    const apiConfig: Configuration = new Configuration({
      basePath: process.env.NEXT_PUBLIC_API || ".",
    });

    const { data: mintPrice } = await new MintsApi(apiConfig).apiMintsPricePost(heroMintPriceRequest);
    return mintPrice;
  }

  public async getHeroMintAuthorization(req: HeroMintOrderReq) {
    const apiConfig: Configuration = new Configuration({
      basePath: process.env.NEXT_PUBLIC_API || ".",
    });

    const { data: heroMintOrder } = await new MintsApi(apiConfig).apiMintsOrderPost(req);
    return heroMintOrder;
  }
}

const mintApi = new MintApi();
export default mintApi;
