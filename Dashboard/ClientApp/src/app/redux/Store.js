import { createStore, applyMiddleware, compose } from 'redux';
import { persistStore, persistReducer } from 'redux-persist';
import thunk from 'redux-thunk';
import Reducer from './reducer/Reducer';
import storage from 'redux-persist/lib/storage'; // defaults to localStorage

const initialState = {}
const persistConfig = {
    key: 'root',
    storage: storage,
    whitelist: ['data'], // elements that will be persisted
    blacklist: [] // elements that will not be persisted
};

let devtools = (x) => x;

if (
    process.env.NODE_ENV !== 'production' &&
    (typeof window) &&
    window.__REDUX_DEVTOOLS_EXTENSION__
) {
    devtools = window.__REDUX_DEVTOOLS_EXTENSION__()
}

const persistedReducer = persistReducer(persistConfig, Reducer);

const store = createStore(persistedReducer, initialState, compose(applyMiddleware(thunk), devtools));
const persistor = persistStore(store);

export { store, persistor };