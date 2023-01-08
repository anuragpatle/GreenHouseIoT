import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';
import { useSelector, useDispatch } from 'react-redux';
import { CO2_DATA } from '../../redux/actions/SustainabilityActions';
import axios from 'axios'
import React from 'react';
import circleSVG from '../../../assets/images/dashboard/circle.svg';
import {predictCo2} from '../../redux/actions/SustainabilityActions'

const Sustainability = () => {
    const dispatch = useDispatch(); // to provide value to selector
    const { output } = useSelector((state) => state.sustainability); // collect the value from dispatch
    const [co2value, setCo2value] = React.useState(0.00);
    var data ={};
     // document on ready
    React.useEffect(() => {
       if(output && output.Results && output.Results.forecast && output.Results.forecast.length>0)
       {
        setCo2value(Number(output.Results.forecast[0]).toFixed(2));
       }
    }, [output]);

    const handleTemperatureChange =(e)=>{
        //setTemperature(parseFloat(e.target.value));
        var date = new Date();
        //date.concat(":00:00.000Z");
        var newDate=new Date(date.setMonth(date.getMonth()+1));
        var forecastedDate=(newDate.toISOString().split(':')[0]).concat(":00:00.000Z");
        data["TimeStamp"]= forecastedDate;
        data["Temperature"] = parseFloat(e.target.value);
      }
      //  const handleHumidityChange =(e)=>{
        //setHumidity(parseFloat(e.target.value));
      //    data["HumidityLevel"] = parseFloat(e.target.value);
     //   }
      //  const handleMoistureChange =(e)=>{
       //   data["MositureLevel"] = parseFloat(e.target.value);
       // }
      //  const handlelightCountChange =(e)=>{
      //    data["LightCount"] = parseFloat(e.target.value);
      //  }
       // const handlelightDurationChange =(e)=>{
       //   data["LightDuration"] = parseFloat(e.target.value);
     // }
    //  const handleCapacityChange =(e)=>{
        //data["LightCapacity"] = parseFloat(e.target.value);
    //    data[e.target.name]= parseFloat(e.target.value);
   //   }

      const handleChange =(e)=>{
        //data["LightCapacity"] = parseFloat(e.target.value);
        if(e.target.name=="Temperature"){ 
          var date = new Date();
          //date.concat(":00:00.000Z");
          var newDate=new Date(date.setMonth(date.getMonth()+1));
          //var oldforecastedDate=(newDate.toISOString().split(':')[0]).concat(":00:00.000Z");
          var forecastedDate=(newDate.toISOString().split('T')[0]).concat("T00:00:00.000Z");
          data["TimeStamp"]= forecastedDate;
        }
       
        //data["Temperature"] = parseFloat(e.target.value);
        data[e.target.name]= parseFloat(e.target.value);
      }
      
      const handleSubmit=(e)=>{
        
        data["Description"] ="example_value";
        data={"Inputs":{"data":[data]}};
        //alert(JSON.stringify(data));
          dispatch(predictCo2(data));
          console.log(JSON.stringify(data));
            e.preventDefault();
            data=undefined;
   
      }
    return (
        <>
        
            <div className="page-header">
                <h3 className="page-title">
                    <span className="page-title-icon bg-gradient-primary text-white mr-1">
                    <i className="mdi mdi-home"></i>
                    </span>
                    Sustainability 
                </h3>
            </div>

            <div className="container">
            <div className="row">
            <div className="col">
            <div className="card bg-gradient-light card-img-holder text-grey" style={{width: "870px"}}>
            <div className="card-body">
            <h5 className="card-title">CO<small>2</small> Emission Prediction</h5>
                        <p className="card-text">Provide your inputs to predict the CO<small>2</small> emission here.</p>
                        <form onSubmit={(e) => {handleSubmit(e)}}>
                            <div className="form-group row">
                            <label className="col-sm-2 col-form-label" >Temperature</label>
                                <div className="col-sm-10">
                                    <input type="" className="form-control" id="inputTemperature" name="Temperature" onChange={(e)=> {handleChange(e)}}  placeholder="Temperature" style={{width:"125px"}}/>
                                </div>
                                <label className="col-sm-2 col-form-label">Humidity</label>
                                <div className="col-sm-10">
                                    <input type="" className="form-control" id="inputHumidity" placeholder="Humidity" name="HumidityLevel"  onChange={(e)=> {handleChange(e)}} style={{width:"125px"}}/>
                                </div>
                                <label  className="col-sm-2 col-form-label">Moisture</label>
                                <div className="col-sm-10">
                                    <input type="" className="form-control" id="inputMoisture" placeholder="Moisture" name="MositureLevel"  onChange={(e)=> {handleChange(e)}} style={{width:"125px"}}/>
                                </div>
                                <label  className="col-sm-2 col-form-label">Light Count</label>
                                <div className="col-sm-10">
                                    <input type="" className="form-control" id="inputLightCount" name="LightCount" placeholder="No. of Lights"  onChange={(e)=> {handleChange(e)}} style={{width:"125px"}} />
                                </div>
                                <label className="col-sm-2 col-form-label">Light Capacity</label>
                                <div className="col-sm-10">
                                    <input type="" className="form-control" id="inputLightCapacity" placeholder="Light Capacity" name="LightCapacity" onChange={(e)=> {handleChange(e)}} style={{width:"125px"}}/>
                                </div>
                                <label className="col-sm-2 col-form-label">Light Duration</label>
                                <div className="col-sm-10">
                                    <input type="" className="form-control" id="inputLightDuration" name="LightDuration" placeholder="Light Duration"  onChange={(e)=> {handleChange(e)}} style={{width:"125px"}}/>
                                </div>
                            </div>
                            <input type="submit" value="Go" className="btn btn-primary"/>
                            </form>
            </div>

            </div>
            </div>
            <div className="col">
                    <div className="card bg-success card-img-holder text-white">
                        <div className="card-body">
                          <h5>CO<small>2</small> Emission </h5>
                          <h3>{co2value}<small> PPM</small></h3>
                        </div>
                    </div>
            
            </div>
            </div>

                </div>
  
        </>
    );
};
export default Sustainability;