import { applyMiddleware, combineReducers, compose, createStore } from "redux";
import thunk from "redux-thunk";
import { AuthReducer } from "./reducers/authReducer";
import { LoadingReducer } from "./reducers/loadingReducer";
const middleware = applyMiddleware(thunk);

const composeEnhancers = window.__REDUX_DEVTOOLS_EXTENSION_COMPOSE__ || compose;

const reducers = combineReducers({
  auth: AuthReducer,
  loading: LoadingReducer,
});

export const store = createStore(reducers, composeEnhancers(middleware));
