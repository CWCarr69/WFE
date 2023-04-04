import { API } from "../../constants";
import { interactWithAPI } from "./base";
import instance from "../../services/axiosInstance";

export const getAudit = async () => interactWithAPI({
  method: "GET",
  url: `${API()}/Audit`,
});

export const exportAudit = async () => {
  var config = {
    method: "GET",
    url: `${API()}/Audit/Export`,
  };

  return new Promise(async (resolve, reject) => {
    await instance(config)
      .then((res) => {
        let values = res.data.split("\r\n");

        values = values.map((v) => v.split(";"));

        const blob = new Blob([values.join("\n")], {
          type: "text/csv",
        });

        // Creating an object for downloading url
        const url = window.URL.createObjectURL(blob);

        // Creating an anchor(a) tag of HTML
        const a = document.createElement("a");

        // Passing the blob downloading url
        a.setAttribute("href", url);

        // Setting the anchor tag attribute for downloading
        // and passing the download file name
        a.setAttribute("download", "download.csv");

        // Performing a download with click
        a.click();
      })
      .catch((err) => {
        console.log(err);
        return err;
      });
  });
};
