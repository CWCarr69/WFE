/// Menu
import Metismenu from "metismenujs";
import React, { Component, useContext, useEffect, useState } from "react";
/// Scroll
import PerfectScrollbar from "react-perfect-scrollbar";
/// Link
import { Link } from "react-router-dom";
import { useScrollPosition } from "@n8tb1t/use-scroll-position";
import { ThemeContext } from "../../context/themeContext";
import { useSelector } from "react-redux";

class MM extends Component {
  componentDidMount() {
    this.$el = this.el;
    this.mm = new Metismenu(this.$el);
  }
  componentWillUnmount() {}
  render() {
    return (
      <div className="mm-wrapper">
        <ul className="metismenu" ref={(el) => (this.el = el)}>
          {this.props.children}
        </ul>
      </div>
    );
  }
}

const SideBar = () => {
  const { iconHover, sidebarposition, headerposition, sidebarLayout } =
    useContext(ThemeContext);
  useEffect(() => {
    var btn = document.querySelector(".nav-control");
    var aaa = document.querySelector("#main-wrapper");
    function toggleFunc() {
      return aaa.classList.toggle("menu-toggle");
    }
    btn.addEventListener("click", toggleFunc);
  }, []);

  const user = useSelector((state) => state.auth.auth);

  // For scroll
  const [hideOnScroll, setHideOnScroll] = useState(true);
  useScrollPosition(
    ({ prevPos, currPos }) => {
      const isShow = currPos.y > prevPos.y;
      if (isShow !== hideOnScroll) setHideOnScroll(isShow);
    },
    [hideOnScroll]
  );

  // let scrollPosition = useScrollPosition();
  /// Path
  let path = window.location.pathname;
  let paths = path.split("/");
  let subPath = paths[paths.length - 1];

  return (
    <div
      className={`deznav ${iconHover} ${
        sidebarposition.value === "fixed" &&
        sidebarLayout.value === "horizontal" &&
        headerposition.value === "static"
          ? hideOnScroll > 120
            ? "fixed"
            : ""
          : ""
      }`}
    >
      <PerfectScrollbar className="deznav-scroll">
        <MM className="metismenu" id="menu">
          <li className={`${path.includes("dashboard") ? "mm-active" : ""}`}>
            <Link to="/dashboard" className="ai-icon">
              <i className="flaticon-025-dashboard"></i>
              <span className="nav-text">Dashboard</span>
            </Link>
          </li>
          <li className={`${path.includes("my-team") ? "mm-active" : ""}`}>
            <Link to="/my-team" className="ai-icon">
              <i className="fa fa-users"></i>
              <span className="nav-text">Timesheets</span>
            </Link>
          </li>
          <li className={`${path.includes("timesheets") ? "mm-active" : ""}`}>
            <Link to={`/timesheets/${user.id}`} className="ai-icon">
              <i className="fa fa-business-time"></i>
              <span className="nav-text">History</span>
            </Link>
          </li>
          {/* <li className={`${path.includes("timeoff") ? "mm-active" : ""}`}>
            <Link to={`/timeoffs/${user.id}`} className="ai-icon">
              <i className="fa fa-calendar"></i>
              <span className="nav-text">Timeoff</span>
            </Link>
          </li> */}
          {/* <li className={`${path.includes("shop&unpaid") ? "mm-active" : ""}`}>
            <Link to={`/shop&unpaid/${user.id}`} className="ai-icon">
              <i className="fa fa-calendar"></i>
              <span className="nav-text">Shop & Unpaid</span>
            </Link>
          </li> */}
          <li className={`${path.includes("holidays") ? "mm-active" : ""}`}>
            <Link to="/holidays" className="ai-icon">
              <i className="fa fa-calendar-times"></i>
              <span className="nav-text">Holidays</span>
            </Link>
          </li>
          <li className={`${path.includes("times-sheets") ? "mm-active" : ""}`}>
            <Link to="/payroll/times-sheets" className="ai-icon">
              <i className="fa fa-cash-register"></i>
              <span className="nav-text">Review Timesheet</span>
            </Link>
          </li>
          {user.isAdministrator && (
            <>
              {/* <li className={`${path.includes("payroll") ? "mm-active" : ""}`}>
                <Link className="has-arrow ai-icon" to="#">
                  <i className="fa fa-cash-register"></i>
                  <span className="nav-text">Payroll</span>
                </Link>
                <ul>
                  <li>
                    <Link
                      className={`${subPath === "logs" ? "mm-active" : ""}`}
                      to="/payroll/times-sheets"
                    >
                      Review Timesheet
                    </Link>
                  </li>
                </ul>
              </li> */}
              <li className={`${path.includes("settings") ? "mm-active" : ""}`}>
                <Link className="has-arrow ai-icon" to="#">
                  <i className="fa fa-tools"></i>
                  <span className="nav-text">Admin</span>
                </Link>
                <ul>
                  <li>
                    <Link
                      className={`${subPath === "general" ? "mm-active" : ""}`}
                      to="/settings/general"
                    >
                      Settings
                    </Link>
                  </li>
                  <li>
                    <Link
                      className={`${
                        subPath === "notifications" ? "mm-active" : ""
                      }`}
                      to="/settings/notifications"
                    >
                      Notifications
                    </Link>
                  </li>
                  <li>
                    <Link to="/settings/audits" className="ai-icon">
                      Audits
                    </Link>
                  </li>
                </ul>
              </li>
            </>
          )}
        </MM>
      </PerfectScrollbar>
    </div>
  );
};

export default SideBar;
