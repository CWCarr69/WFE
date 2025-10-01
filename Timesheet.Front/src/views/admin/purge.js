import React, { useState } from 'react';
import { toast } from "react-toastify";
import { purgeTables } from '../../redux/actions/purge';

const Purge = () => {
  const [isPurging, setIsPurging] = useState(false);
  const [result, setResult] = useState(null);

  const handlePurge = async () => {
    setIsPurging(true);
    setResult(null);
    await purgeTables()
      .then((resp) => {
        setResult(resp.result);
      })
      .catch((err) => {
        toast.error(err?.response.data.message ? err.response.data.message : 'Purge failed. Please try again.');
      })
      .finally((tmp) => {
        setIsPurging(false);
      });
  };

  return (
    <div className="admin-purge">
      <h2>Purge Data</h2>
      <p>
        This action will permanently delete selected data from the system. 
        Please proceed with caution.
      </p>
      <button className="btn btn-danger btn-sm" onClick={handlePurge} disabled={isPurging}>
        {isPurging ? 'Purging...' : 'Purge'}
      </button>
      {result && <div className="purge-result">{result}</div>}
    </div>
  );
};

export default Purge;