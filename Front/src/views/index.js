import React, { useContext } from "react";

/// React router dom
import { Switch, Route } from "react-router-dom";

/// Css
import "../assets/css/index.css";
import "../assets/css/chart.css";
import "../assets/css/step.css";

/// Layout
import Nav from "../layouts/nav";
import Footer from "../layouts/footer";
import ScrollToTop from "../layouts/scrollToTop";

/// Dashboard
import Home from "./dashboard";

/// Auhtentication Pages
// import Registration from "./pages/Registration";
import Login from "./login";

import { ThemeContext } from "../context/themeContext";
import Profile from "./profile/profile";
import Setting from "../layouts/setting";
import EmployeeList from "./team/employeeList";
import Holidays from "./holidays/holidays";
import Audit from "./audits/audit";
import Connexions from "./configs/connexions";
import Notifications from "./configs/notifications";
import TimesSheet from "./comptabilite/timesSheet";
import Timesheets from "./timesheet/timesheets";
import TimeOffs from "./timeoff/timeOffs";
import TimeOff from "./timeoff/timeoff";
import ShopUnpaids from "./shopUnpaid/timeOffs";
import ShopUnpaid from "./shopUnpaid/timeoff";
import Timesheet from "./timesheet/timesheet";
import { ToastContainer } from "react-toastify";
import NewEntry from "./timesheet/newEntry";

const Markup = () => {
  const { menuToggle } = useContext(ThemeContext);
  // const loading = useSelector((state) => state.loading.showLoading);
  const routes = [
    /// Dashboard
    { url: "", component: Home },
    { url: "dashboard", component: Home },

    /// Profile
    { url: "profile/:id", component: Profile },

    /// MY TEAM
    { url: "my-team", component: EmployeeList },

    /// DAYSOFFS
    { url: "holidays", component: Holidays },

    /// TIMESHEET
    { url: "timesheets/:id", component: Timesheets },
    { url: "timesheets/:id/entry", component: NewEntry },
    { url: "timesheets/:id/employee/:employee/:date?", component: Timesheet },

    /// TIMEOFF
    { url: "timeoffs/:id", component: TimeOffs },
    { url: "timeoffs/:id/employee/:employee/:date?", component: TimeOff },

    /// SHOP & UNPAID
    { url: "shop&unpaid/:id", component: ShopUnpaids },
    { url: "shop&unpaid/:id/employee/:employee/:date?", component: ShopUnpaid },

    /// COMPTABILITE
    { url: "payroll/times-sheets", component: TimesSheet },

    /// CONFIGS
    { url: "settings/general", component: Connexions },
    { url: "settings/notifications", component: Notifications },
    { url: "settings/audits", component: Audit },

    /// Auhtentication Pages
    // { url: "page-register", component: Registration },
    { url: "page-login", component: Login },
  ];
  let path = window.location.pathname;
  path = path.split("/");
  path = path[path.length - 1];

  let pagePath = path.split("-").includes("page");
  return (
    <>
      <ToastContainer
        position="top-right"
        autoClose={5000}
        hideProgressBar={false}
        newestOnTop
        closeOnClick
        rtl={false}
        pauseOnFocusLoss
        draggable
        pauseOnHover
        style={{
          width: "auto",
        }}
      />
      <>
        <div
          id={`${!pagePath ? "main-wrapper" : ""}`}
          className={`${!pagePath ? "show" : "mh100vh"}  ${
            menuToggle ? "menu-toggle" : ""
          }`}
        >
          {!pagePath && <Nav />}

          <div className={`${!pagePath ? "content-body" : ""}`}>
            <div
              className={`${!pagePath ? "container-fluid" : ""}`}
              style={{ minHeight: window.screen.height - 60 }}
            >
              <Switch>
                {routes.map((data, i) => (
                  <Route
                    key={i}
                    exact
                    path={`/${data.url}`}
                    component={data.component}
                  />
                ))}
              </Switch>
            </div>
          </div>
          {!pagePath && <Footer />}
        </div>
        <Setting />
        <ScrollToTop />
      </>
    </>
  );
};

export default Markup;
