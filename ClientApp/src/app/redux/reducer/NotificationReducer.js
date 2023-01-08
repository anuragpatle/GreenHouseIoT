import { FAN_STATUS_NOTIFICATION, DE_HUMIDIFIER_STATUS_NOTIFICATION, LIGHTNING_STATUS_NOTIFICATION, WATER_PUMP_STATUS_NOTIFICATION } from '../actions/NotificationActions';

const initialState = {
    fanStatus: false,
    deHumidifiertatus: false,
    lightningStatus: false,
    waterPumpStatus: false
};

const NotificationReducer = function (state = initialState, action) {
    switch (action.type) {
        case FAN_STATUS_NOTIFICATION: {
            return {
                ...state,
                fanStatus: action.status
            };
        }
        case DE_HUMIDIFIER_STATUS_NOTIFICATION: {
            return {
                ...state,
                deHumidifiertatus: action.status
            };
        }
        case LIGHTNING_STATUS_NOTIFICATION: {
            return {
                ...state,
                lightningStatus: action.status
            };
        }
        case WATER_PUMP_STATUS_NOTIFICATION: {
            return {
                ...state,
                waterPumpStatus: action.status
            };
        }
        default: {
            return {
                ...state,
            };
        }
    };
};

export default NotificationReducer;