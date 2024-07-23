import { toast } from "react-toastify";

export const displayError = (err, defaultMessage) => {
    toast.error(
        err?.response.data.message ? err.response.data.message : defaultMessage,
        {
            position: "top-right",
            autoClose: 2000,
            hideProgressBar: false,
            closeOnClick: true,
            pauseOnHover: true,
            draggable: true,
            progress: undefined,
          }
    );
}

export const displaySuccess = (message) => {
    toast.success(message, {
        position: "top-right",
        autoClose: 5000,
        hideProgressBar: false,
        closeOnClick: true,
        pauseOnHover: true,
        draggable: true,
    });
}