import axiosInstance from '../../util/Axios'

export const GET_PREDICTION_CO2 = 'GET_PREDICTION_CO2'

export const predictCo2 = (obj) => (dispatch) => {
    axiosInstance.post('/score', obj ).then((res) => {
        console.log(res.data)
        dispatch({
            type: GET_PREDICTION_CO2,
            output: res.data,
        })
    })
}