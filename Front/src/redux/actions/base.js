import instance from "../../services/axiosInstance";

export const interactWithAPI = async (config) => {  
    return new Promise(async (resolve, reject) => {
      await instance(config)
        .then((res) => {
          resolve(res.data);
        })
        .catch((err) => {
          reject(err);
        });
    });
  };