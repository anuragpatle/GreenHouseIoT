import React from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { toast } from "react-toastify";
import circleSVG from '../../../assets/images/dashboard/circle.svg';
import { HubConnectionBuilder } from '@microsoft/signalr';
import { LOAD_DATA } from '../../redux/actions/DashboardActions';
import { FAN_STATUS_NOTIFICATION, DE_HUMIDIFIER_STATUS_NOTIFICATION, LIGHTNING_STATUS_NOTIFICATION, WATER_PUMP_STATUS_NOTIFICATION } from '../../redux/actions/NotificationActions';
import LineChart from '../../component/Charts/LineChart';
import BootstrapSwitchButton from 'bootstrap-switch-button-react';
import { FaRegFrownOpen, FaRegGrinBeamSweat, FaRegGrinBeam, FaRegGrinStars } from "react-icons/fa";
// import { makeFanOFF, makeFanON } from './SimulatedDevice';
// import { Client } from 'azure-iothub';
import axios from 'axios'; 
import d from  "../../../assets/images/3d.jpg" 

const Dashboard = () => {
    const dispatch = useDispatch();

    const [signalRHubConnection, setSignalRHubConnection] = React.useState(null);
    const { data } = useSelector((state) => state.dashboard);
    const { fanStatus, deHumidifiertatus, lightningStatus, waterPumpStatus } = useSelector((state) => state.notification);
    const iotUrl = "http://localhost:80/";


    const callOperateIoTDevices=(message)=>{
        axios.get(iotUrl+''+message)
            .then(response => console.log(response.data));
    }

    //TODO: IOTHUB api needs to call in this function
    const setFlagToIotDashBoard = (flag) => {
        console.log('flag value', flag);
    }
    const handleChange = (e) => {
        const isFanFlagOn = e;
        console.log("e: ", e)
        console.log("fanStatus: ", fanStatus)
        setFlagToIotDashBoard(isFanFlagOn)
        if (fanStatus) {
            callOperateIoTDevices("makeFanOFF")
        } else {
            callOperateIoTDevices("makeFanON")
        }
    }

    const handleWaterpumpChange = (e) => {

        console.log("waterPumpStatus: ", waterPumpStatus)
        if (waterPumpStatus) {
            callOperateIoTDevices("makeWaterpumpOFF")
        } else {
            callOperateIoTDevices("makeWaterpumpON")
        }
    }

    // React.useEffect(() => {
    //     const newConnection = new HubConnectionBuilder()
    //         .withUrl(process.env.REACT_APP_SIGNAL_R_API)
    //         .build();
    //     setSignalRHubConnection(newConnection);
    // }, []);

    React.useEffect(() => {
        const newConnection = new HubConnectionBuilder()
            .withUrl("https://fa-greenhouse.azurewebsites.net/api")
            .build();
        setSignalRHubConnection(newConnection);
    }, []);

    React.useEffect(() => {
        if (signalRHubConnection) {
            signalRHubConnection.start().then(result => {
                signalRHubConnection.on('newMessage', message => {
                    const data = JSON.parse(message);
                    dispatch({
                        type: LOAD_DATA,
                        data: data
                    });
                    dispatch({
                        type: FAN_STATUS_NOTIFICATION,
                        status: data.fanStatus
                    });
                    dispatch({
                        type: DE_HUMIDIFIER_STATUS_NOTIFICATION,
                        status: data.dehumidifierStatus
                    });
                    dispatch({
                        type: LIGHTNING_STATUS_NOTIFICATION,
                        status: data.lightingStatus
                    });
                    dispatch({
                        type: WATER_PUMP_STATUS_NOTIFICATION,
                        status: data.waterpumpStatus
                    });
                });
            }).catch(e => {
                toast.error("SignalR Hub Connection Error " + e);
            });
        }
    }, [signalRHubConnection, dispatch]);
    let predictivePercentage = '';

    if (data.predictivePercentage >= 75) { predictivePercentage = 'Excellent' }
    else if (data.predictivePercentage >= 50) { predictivePercentage = 'Good' }
    else if (data.predictivePercentage >= 25) { predictivePercentage = 'Average' }
    else if (data.predictivePercentage >= 0) { predictivePercentage = 'Poor' }
    return (
        <>
            <div className="page-header">
                <h3 className="page-title">
                    <span className="page-title-icon bg-gradient-primary text-white mr-2">
                        <i className="mdi mdi-home"></i>
                    </span>
                    Dashboard
                </h3>
            </div>
            <div className="row">
                <div className="col-xs-3 col-sm-3 col-md-3 col-lg-3 col-xl-3">
                    <div className="card bg-gradient-danger card-img-holder text-white">
                        <div className="card-body">
                            <img src={circleSVG} className="card-img-absolute" alt="circle" />
                            <h4 className="font-weight-normal mb-3">Temperature <i className="mdi mdi-thermometer-lines mdi-24px float-right"></i>
                            </h4>
                            <h2>{data.temperature}<span>&#8451;</span></h2>
                        </div>
                    </div>
                </div>
                <div className="col-xs-3 col-sm-3 col-md-3 col-lg-3 col-xl-3">
                    <div className="card bg-gradient-info card-img-holder text-white">
                        <div className="card-body">
                            <img src={circleSVG} className="card-img-absolute" alt="circle" />
                            <h4 className="font-weight-normal mb-3">Humidity Value<i className="mdi mdi-trending-down mdi-24px float-right text-full-danger"></i>
                            </h4>
                            <h2>{data.humidityLevel} <small> Units</small></h2>
                        </div>
                    </div>
                </div>
                <div className="col-xs-3 col-sm-3 col-md-3 col-lg-3 col-xl-3">
                    <div className="card bg-gradient-success card-img-holder text-white">
                        <div className="card-body">
                            <img src={circleSVG} className="card-img-absolute" alt="circle" />
                            <h4 className="font-weight-normal mb-3">CO<small>2</small> Level <i className="mdi mdi-fire mdi-24px float-right"></i>
                            </h4>
                            <h2>{data.co2Level}<small>ppm</small></h2>
                        </div>
                    </div>
                </div>
                <div className="col-xs-3 col-sm-3 col-md-3 col-lg-3 col-xl-3">
                    <div className="card bg-gradient-warning card-img-holder text-white">
                        <div className="card-body">
                            <img src={circleSVG} className="card-img-absolute" alt="circle" />
                            <h4 className="font-weight-normal mb-3">Moisture Value<i className="mdi mdi-air-filter  mdi-24px float-right text-chocolate"></i>
                            </h4>
                            <h2>{data.mositureLevel}<small> Units</small></h2>
                        </div>
                    </div>
                </div>
            </div>
            {data && <div className="row mt-3">
                <div className="col-xs-6 col-sm-6 col-md-6 col-lg-6 col-xl-6">
                    <LineChart title="Temperature and Humidity (Hourly)" data={data} />
                </div>

                <div className="col-xs-6 col-sm-6 col-md-6 col-lg-6 col-xl-6">
                    <div className="row">
                        <div className="card" style={{ width: '100%', margin: '0px 15px 30px 0px' }}>
                            <div className="card-body">
                                <div className="clearfix mb-4">
                                    <h4 className="card-title float-left">Yield Prediction Meter : {data.predictivePercentage}
                                        {data.predictivePercentage > 0 ? '%' : ''}
                                    </h4>
                                </div>
                                <div className="row">
                                    <div className="col-xs-2 col-sm-2 col-md-3 col-lg-2 col-xl-2">
                                        <FaRegFrownOpen size="1.8em" color={data.predictivePercentage >= 0 ? '#e20074' : 'grey'} ></FaRegFrownOpen>
                                    </div>
                                    <div className="col-xs-2 col-sm-2 col-md-3 col-lg-2 col-xl-2">
                                        <FaRegGrinBeamSweat size="1.8em" color={data.predictivePercentage >= 25 ? '#e20074' : 'grey'} ></FaRegGrinBeamSweat>
                                    </div>
                                    <div className="col-xs-2 col-sm-2 col-md-3 col-lg-2 col-xl-2">
                                        <FaRegGrinBeam size="1.8em" color={data.predictivePercentage >= 50 ? '#e20074' : 'grey'} ></FaRegGrinBeam>
                                    </div>
                                    <div className="col-xs-2 col-sm-2 col-md-3 col-lg-2 col-xl-2">
                                        <FaRegGrinStars size="1.8em" color={data.predictivePercentage >= 75 && data.predictivePercentage < 101 ? '#e20074' : 'grey'} ></FaRegGrinStars>
                                    </div>
                                    <div >
                                        <span style={{ color: '#e20074', fontWeight: '500', fontSize: '20px' }}>{predictivePercentage}</span>
                                    </div>

                                </div>
                            </div>
                        </div>

                        <div className="card" style={{backgroundImage: `url(${d})`, backgroundPosition: "center"}}>
                            <div className="card-body">
                                <div className="clearfix mb-4">
                                    <h4 className="card-title float-left">Smart Device Activities</h4>
                                </div>
                                <div className="row">
                                    <div className="col-xs-6 col-sm-6 col-md-6 col-lg-6 col-xl-6">
                                        <BootstrapSwitchButton
                                            checked={fanStatus}
                                            onlabel='Fan is on'
                                            offlabel='Fan is off'
                                            onstyle="danger"
                                            width={235}
                                            height={50}
                                            onChange={handleChange}
                                        />
                                    </div>
                                    <div className="col-xs-6 col-sm-6 col-md-6 col-lg-6 col-xl-6">
                                        <BootstrapSwitchButton
                                            checked={waterPumpStatus}
                                            onlabel='Sprinkler is on'
                                            offlabel='Sprinkler is off'
                                            onstyle="danger"
                                            width={235}
                                            height={50}
                                            onChange={handleWaterpumpChange}
                                        />
                                    </div>
                                </div>
                                {/* <div className="row mt-3">
                                    <div className="col-xs-6 col-sm-6 col-md-6 col-lg-6 col-xl-6">
                                        <BootstrapSwitchButton
                                            checked={lightningStatus}
                                            onlabel='Light is on'
                                            offlabel='Light is off'
                                            onstyle="light"
                                            width={235}
                                            height={50}
                                            disabled={true}
                                        />
                                    </div>
                                    <div className="col-xs-6 col-sm-6 col-md-6 col-lg-6 col-xl-6">
                                        <BootstrapSwitchButton
                                            checked={waterPumpStatus}
                                            onlabel='Water Sprinkler is on'
                                            offlabel='Water Sprinkler is off'
                                            onstyle="warning"
                                            width={303}
                                            height={50}
                                            disabled={true}
                                        />
                                    </div>
                                </div> */}
                            </div>
                        </div>
                    </div>
                    <div className="row mt-3">
                        {
                            data && data.scheduleInfo && data.scheduleInfo.map((scObj, index) => (
                                <div key={index} className="col-xs-6 col-sm-6 col-md-6 col-lg-6 col-xl-6" style={{ paddingLeft: '0' }}>
                                    <div className="card">
                                        <div className="card-body">
                                            <div className="clearfix mb-4">
                                                <h4 className="card-title float-left">{scObj.scheduleFor} Schedule</h4>
                                            </div>
                                            <div className="table-responsive">
                                                <table className="table">
                                                    <thead>
                                                        <tr>
                                                            <th>Start Time</th>
                                                            <th>End Time</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <tr>
                                                            <td>{scObj.scheduleStarttime}</td>
                                                            <td>{scObj.scheduleFinishtime}</td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            ))
                        }
                    </div>
                </div>
            </div>
            }
        </>
    );
};
export default Dashboard;
