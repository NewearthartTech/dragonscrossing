import axios from "axios";
import { isBrowser } from "core/utils";
import { HocProps } from "./server-side-props";
import Verification from "./verification";
import { Configuration } from "@dcx/dcx-backend";

export function withHoc(Component: any) {
  // const hoc = (props: HocProps) => {
  //   const { api } = props;
  const hoc = () => {
    //dee: process.env.API gets set to NEXT_PUBLIC_API at compile time. and not available at run time
    //so we need to use the runtime variable here
    //const api = process.env.API || "";
    const api = process.env.NEXT_PUBLIC_API || ".";
    if (isBrowser()) {
      axios.defaults.baseURL = api;
    }
    return <Verification comp={Component} api={api} />;
  };
  return hoc;
}
