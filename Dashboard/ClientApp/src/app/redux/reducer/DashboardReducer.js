import { LOAD_DATA } from '../actions/DashboardActions';

const initialState = {
    data: {
        co2Level: 0,
        temperature: 0,
        humidityLevel: 0,
        mositureLevel: 0,
        xAxisData: [],
        tempatureSeriesData: [],
        humiditySeriesData: []
    }
};

const DashboardReducer = function (state = initialState, action) {
    switch (action.type) {
        case LOAD_DATA: {
            return {
                ...state,
                data: { ...action.data }
            };
        }
        default: {
            return {
                ...state,
            };
        }
    };
};

export default DashboardReducer;