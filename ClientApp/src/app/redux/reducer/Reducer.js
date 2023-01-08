import { combineReducers } from 'redux';
import DashboardReducer from './DashboardReducer';
import NotificationReducer from './NotificationReducer';
import SustainabilityReducer from './SustainabilityReducer';

const Reducer = combineReducers({
    dashboard: DashboardReducer,
    notification: NotificationReducer,
    sustainability:SustainabilityReducer
});

export default Reducer;
