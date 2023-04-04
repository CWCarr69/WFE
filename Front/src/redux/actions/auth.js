import { toast } from "react-toastify";
import {
  formatError,
  login,
  saveTokenInLocalStorage,
} from "../../services/authService";
import TokenService from "../../services/tokenService";

export const LOGIN_CONFIRMED_ACTION = "[login action] confirmed login";
export const LOGIN_FAILED_ACTION = "[login action] failed login";
export const LOADING_TOGGLE_ACTION = "[Loading action] toggle loading";
export const LOGOUT_ACTION = "[Logout action] logout action";

export function logout(history) {
  localStorage.removeItem("userDetails");
  history.push("/");
  return {
    type: LOGOUT_ACTION,
  };
}

export function loginAction(email, password, history) {
  return (dispatch) => {
    login(email, password)
      .then((response) => {
        let exp = TokenService.decode(response.data.token).exp;
        saveTokenInLocalStorage({
          token: response.data.token,
          ...response.data.user,
          expiresIn: exp,
        });
        // runLogoutTimer(dispatch, exp * 1000, history);
        dispatch(
          loginConfirmedAction({
            token: response.data.token,
            ...response.data.user,
          })
        );
        history.push("/dashboard");
      })
      .catch((error) => {
        console.log(error);
        const errorMessage = formatError(error?.response?.data);
        dispatch(loginFailedAction(errorMessage));
        toast.error(`${error.response.data}`, {
          position: "top-right",
          autoClose: 5000,
          hideProgressBar: false,
          closeOnClick: true,
          pauseOnHover: true,
          draggable: true,
          progress: undefined,
        });
      });
  };
}

export function loginFailedAction(data) {
  return {
    type: LOGIN_FAILED_ACTION,
    payload: data,
  };
}

export function loginConfirmedAction(data) {
  return {
    type: LOGIN_CONFIRMED_ACTION,
    payload: data,
  };
}

export function loadingToggleAction(status) {
  return {
    type: LOADING_TOGGLE_ACTION,
    payload: status,
  };
}
