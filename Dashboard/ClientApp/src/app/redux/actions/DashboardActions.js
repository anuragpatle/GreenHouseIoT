
export const LOAD_DATA = 'LOAD_DATA';

export const loadData = () => (dispatch) => {
    dispatch({
        type: LOAD_DATA,
        payload: [],
    });
};
