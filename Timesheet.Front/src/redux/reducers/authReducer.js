import {
  LOADING_TOGGLE_ACTION,
  LOGIN_CONFIRMED_ACTION,
  LOGIN_FAILED_ACTION,
  LOGOUT_ACTION,
} from "../actions/auth";

let user = JSON.parse(localStorage.getItem("userDetails"));

const initialState = {
  auth: user
    ? user
    : {
        email: "",
        idToken: "",
        localId: "",
        expiresIn: "",
        refreshToken: "",
      },
  errorMessage: "",
  successMessage: "",
  showLoading: false,
};

export function AuthReducer(state = initialState, action) {
  if (action.type === LOGIN_CONFIRMED_ACTION) {
    return {
      ...state,
      auth: action.payload,
      errorMessage: "",
      successMessage: "Login Successfully Completed",
      showLoading: false,
    };
  }

  if (action.type === LOGOUT_ACTION) {
    return {
      ...state,
      errorMessage: "",
      successMessage: "",
      auth: {
        email: "",
        idToken: "",
        localId: "",
        expiresIn: "",
        refreshToken: "",
      },
    };
  }

  if (action.type === LOGIN_FAILED_ACTION) {
    return {
      ...state,
      errorMessage: action.payload,
      successMessage: "",
      showLoading: false,
    };
  }

  if (action.type === LOADING_TOGGLE_ACTION) {
    return {
      ...state,
      showLoading: action.payload,
    };
  }
  return state;
}
