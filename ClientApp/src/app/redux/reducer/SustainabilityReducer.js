import {GET_PREDICTION_CO2} from '../actions/SustainabilityActions'

const initialState = {
    output: {

    }
};

const SustainabilityReducer = function (state = initialState, action) {
    switch (action.type) {
        case GET_PREDICTION_CO2: {
            return {
                ...state,
                output: { ...action.output }
            };
        }
        default: {
            return {
                ...state,
            };
        }
    };
};

export default SustainabilityReducer;