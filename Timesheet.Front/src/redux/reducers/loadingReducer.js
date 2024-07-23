const initialState = {
  showLoading: false,
};

export function LoadingReducer(state = initialState, action) {
  if (action.type === "LOADING") {
    return {
      showLoading: action.payload,
    };
  }
  return state;
}
