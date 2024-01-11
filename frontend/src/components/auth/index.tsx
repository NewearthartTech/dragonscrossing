import React, { useState, useCallback, useEffect, useRef, useMemo } from "react";
import constate from "constate";
import { AuthenticatedUser } from "@dcx/dcx-backend";
import { AuthDlg } from "./authDlg";
import { unAuthHandler } from "../hoc/apiErrors";

export const [LoginProvider, useLoginDlg, useAuthentication] = constate(
  useLogin,
  (v) => v.LoginDlg,
  (v) => v.login
);

const LOGIN_KEY_NAME = "login_key";

function loadPKey() {
  if (typeof localStorage === "undefined") return;

  const value = localStorage.getItem(LOGIN_KEY_NAME);

  if (!!value) {
    const recovered: AuthenticatedUser = JSON.parse(value);

    if (recovered?.jwt) {
      //setAuthenticatedUser(recovered);
      //loginResolverRef.current.resolve(recovered);
      return recovered;
    }
  }
}

function useLogin() {
  const [authenticatedUser, setAuthenticatedUser] = useState<AuthenticatedUser | undefined>(loadPKey());

  const [visible, setVisible] = useState<boolean>();

  const loginResolverRef = useRef<{
    resolve: (d: AuthenticatedUser) => void;
    reject: (r?: any) => void;
    thePromise?: Promise<AuthenticatedUser>;
  }>();

  async function savedKeyStore(stored?: AuthenticatedUser) {
    if (stored) {
      localStorage.setItem(LOGIN_KEY_NAME, JSON.stringify({ ...stored, savedAt: new Date() }));
    } else {
      await localStorage.removeItem(LOGIN_KEY_NAME);
    }
  }

  useEffect(() => {
    unAuthHandler.onUnAuthorized = () => {
      signOut();
    };
  }, []);

  const signOut = () => {
    loginResolverRef.current?.reject();
    loginResolverRef.current = undefined;

    setAuthenticatedUser(undefined);
    savedKeyStore(undefined);
  };

  const updateJWT = (user: AuthenticatedUser) => {
    setAuthenticatedUser(user);
    savedKeyStore(user);
  };

  const LoginDlg = useCallback(() => {
    //if (authenticatedUser)
    //    return null;

    if (!visible) return null;

    return (
      <AuthDlg
        currentUser={authenticatedUser}
        onSignIn={(d, isAuthenticated) => {
          setAuthenticatedUser(d);
          savedKeyStore(d);

          if (isAuthenticated) {
            loginResolverRef.current?.resolve(d);
            loginResolverRef.current = undefined;
            setVisible(false);
          }
        }}
        onCancel={(error: any) => {
          loginResolverRef.current?.reject(error);
          loginResolverRef.current = undefined;

          //loginPromiseRef.current = undefined;

          setVisible(false);
        }}
      />
    );
  }, [authenticatedUser, loginResolverRef.current]);

  const ensureLogin = (claim: "wallet") => {
    if (authenticatedUser) {
      let hasClaim = false;

      switch (claim) {
        case "wallet":
          hasClaim = !!authenticatedUser.player?.blockchainPublicAddress;
      }

      if (hasClaim) return Promise.resolve(authenticatedUser);
    }

    /*
        if (loginResolverRef.current?.thePromise)
            return loginResolverRef.current?.thePromise;
*/
    const thePromise = new Promise<AuthenticatedUser>((resolve, reject) => {
      loginResolverRef.current = { resolve, reject };
    });

    setVisible(true);

    //loginPromiseRef.current = t;

    if (loginResolverRef.current) {
      loginResolverRef.current = { ...loginResolverRef.current, thePromise };
    } else {
      console.error("loginResolverRef.current should not be empty");
    }
    return thePromise;
  };

  const login = useMemo(
    () => ({
      authenticatedUser,
      ensureLogin,
      signOut,
      updateJWT,
    }),
    [authenticatedUser, loginResolverRef.current]
  );

  return { LoginDlg, login };
}
