import { lazy, Suspense, useEffect } from "react";

/// Components
import Index from "./views";
import { useSelector } from "react-redux";
import { Route, Switch, withRouter } from "react-router-dom";
// action
import { isAuthenticated } from "./redux/selectors/authSelectors";
/// Style
import "./assets/vendor/bootstrap-select/dist/css/bootstrap-select.min.css";
import "./assets/css/style.css";
import SpinnerComponent from "./components/spinner/spinner";

const Login = lazy(() => {
  return new Promise((resolve) => {
    setTimeout(() => resolve(import("./views/login")), 500);
  });
});

const App = () => {
  const token = useSelector((state) => state.auth.auth.token);

  let routes = (
    <Switch>
      <Route path="/" component={Login} />
    </Switch>
  );

  return token ? (
    <Suspense
      fallback={
        <div id="preloader">
          <div className="sk-three-bounce">
            <div className="sk-child sk-bounce1"></div>
            <div className="sk-child sk-bounce2"></div>
            <div className="sk-child sk-bounce3"></div>
          </div>
        </div>
      }
    >
      <Index />
    </Suspense>
  ) : (
    <div className="vh-100">
      <Suspense
        fallback={
          <div id="preloader">
            <div className="sk-three-bounce">
              <div className="sk-child sk-bounce1"></div>
              <div className="sk-child sk-bounce2"></div>
              <div className="sk-child sk-bounce3"></div>
            </div>
          </div>
        }
      >
        {routes}
      </Suspense>
    </div>
  );
};

export default App;
