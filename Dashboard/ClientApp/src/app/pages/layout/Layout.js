import React from 'react';
import { Route, Routes, Navigate } from 'react-router-dom';
import Navbar from '../../component/Navbar/Navbar';
import Sidebar from '../../component/Sidebar/Sidebar';
import { toast, ToastContainer } from "react-toastify";
import { useSelector } from 'react-redux';

const Dashboard = React.lazy(() => import('../dashboard/Dashboard'));
const Sustainability = React.lazy(() => import('../sustainability/Sustainability'));

const Layout = () => {
    const [oldFanStatus, setOldFanStatus] = React.useState(false);
    const [oldDeHumidifiertatus, setOldDeHumidifiertatus] = React.useState(false);
    const [oldLightningStatus, setOldLightningStatus] = React.useState(false);
    const [oldWaterPumpStatus, setOldWaterPumpStatus] = React.useState(false);

    const { fanStatus, deHumidifiertatus, lightningStatus, waterPumpStatus } = useSelector((state) => state.notification);
   
    React.useEffect(() => {
        if (oldFanStatus !== fanStatus) {
            setOldFanStatus(fanStatus);
            toast.info("Fan is turned " + (fanStatus ? "On" : "Off"));
        }

        if (oldDeHumidifiertatus !== deHumidifiertatus) {
            setOldDeHumidifiertatus(deHumidifiertatus);
            toast.info("De-humidifier is turned " + (deHumidifiertatus ? "On" : "Off"));
        }

        if (oldLightningStatus !== lightningStatus) {
            setOldLightningStatus(lightningStatus);
            toast.info("Light is turned " + (lightningStatus ? "On" : "Off"));
        }

        if (oldWaterPumpStatus !== waterPumpStatus) {
            setOldWaterPumpStatus(waterPumpStatus);
            toast.info("Water Sprinkler is turned " + (waterPumpStatus ? "On" : "Off"));
        }
    }, [fanStatus, deHumidifiertatus, lightningStatus, waterPumpStatus, oldFanStatus, oldWaterPumpStatus, oldLightningStatus, oldDeHumidifiertatus]);

    return (
        <div className="container-scroller">
            <Navbar />
            <ToastContainer autoClose={5000} position='top-center' hideProgressBar={false} rtl={false} />
            <div className="container-fluid page-body-wrapper">
                <Sidebar />
                <div className="main-panel">
                    <div className="content-wrapper">
                        <Routes>
                            <Route
                                path='/'
                                element={<Navigate to="/dashboard" replace={true} />}
                            />
                            <Route
                                path='/dashboard'
                                element={<Dashboard />}
                            />
                            <Route
                                path='/sustainability'
                                element={<Sustainability />}
                            />
                        </Routes>
                    </div>
                </div>
            </div>
        </div>
    );
};
export default Layout;