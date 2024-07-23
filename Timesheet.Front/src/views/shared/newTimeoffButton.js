import React from "react";
const NewTimeoffButton = ({ onClick }) => {
  return (
    <span className="badge badge-rounded badge-primary" style={{ cursor: "pointer", marginLeft: 10 }} onClick={onClick}>
    Add
    </span>
  );
};

export default NewTimeoffButton;
