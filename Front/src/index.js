import React from "react";
import ReactDOM from "react-dom";
import App from "./app";
import { BrowserRouter } from "react-router-dom";
import { Provider } from "react-redux";
import { store } from "./redux/store";
import reportWebVitals from "./reportWebVitals";
import SimpleReactLightbox from "simple-react-lightbox";
import ThemeContext from "./context/themeContext";

ReactDOM.render(
  <Provider store={store}>
    <SimpleReactLightbox>
      <BrowserRouter basename="">
        <ThemeContext>
          <App />
        </ThemeContext>
      </BrowserRouter>
    </SimpleReactLightbox>
  </Provider>,
  document.getElementById("root")
);
reportWebVitals();
