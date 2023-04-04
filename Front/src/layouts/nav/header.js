import React, { useContext, useState } from "react";

import { Link } from "react-router-dom";
/// Scroll
import PerfectScrollbar from "react-perfect-scrollbar";
import { Dropdown } from "react-bootstrap";

/// Image
import profile from "../../assets/images/profile/pic1.jpg";
import avatar from "../../assets/images/avatar/1.jpg";

import LogoutPage from "./logout";
import { ThemeContext } from "../../context/themeContext";
import { useSelector } from "react-redux";

const Header = ({ onNote }) => {
  const [country, setCountry] = useState("ENGLISH");

  const { title } = useContext(ThemeContext);

  const user = useSelector((state) => state.auth.auth);

  return (
    <div className="header">
      <div className="header-content">
        <nav className="navbar navbar-expand">
          <div className="collapse navbar-collapse justify-content-between">
            <div className="header-left">
              <h2 className="font-w600 mb-0">{title}</h2>
            </div>
            <ul className="navbar-nav header-right main-notification">
              <Dropdown as="li" className="nav-item  notification_dropdown " />
              <Dropdown
                as="li"
                className="nav-item dropdown notification_dropdown "
              />

              <Dropdown
                as="li"
                className="nav-item dropdown notification_dropdown "
              />

              <Dropdown as="li" className="nav-item dropdown header-profile">
                <Dropdown.Toggle
                  variant=""
                  as="a"
                  className="nav-link i-false c-pointer"
                  role="button"
                  data-toggle="dropdown"
                >
                  <div className="header-info me-3">
                    <span className="fs-16 font-w600 ">{user.fullname}</span>
                    <small className="text-end fs-14 font-w400">
                      {user.isAdministrator ? "Administrator" : "Employee"}
                    </small>
                  </div>
                </Dropdown.Toggle>
                <Dropdown.Menu align="right" className="mt-0 dropdown-menu-end">
                  <Link
                    to={`/profile/${user.id}`}
                    className="dropdown-item ai-icon"
                  >
                    <svg
                      id="icon-user1"
                      xmlns="http://www.w3.org/2000/svg"
                      className="text-primary"
                      width={18}
                      height={18}
                      viewBox="0 0 24 24"
                      fill="none"
                      stroke="currentColor"
                      strokeWidth={2}
                      strokeLinecap="round"
                      strokeLinejoin="round"
                    >
                      <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2" />
                      <circle cx={12} cy={7} r={4} />
                    </svg>
                    <span className="ms-2">Profile</span>
                  </Link>
                  <LogoutPage />
                </Dropdown.Menu>
              </Dropdown>
            </ul>
          </div>
        </nav>
      </div>
    </div>
  );
};

export default Header;
