import React from 'react';
import AppContext from './context/AppContext';
import { Provider } from 'react-redux';
import { PersistGate } from 'redux-persist/integration/react';
import { persistor, store } from './redux/Store';
import { HashRouter as Router } from 'react-router-dom';
import history from './util/history'
import TsinSuspense from './component/TsinSuspense/TsinSuspense';
import routes from './util/routes/Routes';

const Layout = React.lazy(() => import('./pages/layout/Layout'));


const App = () => {
    return (
        <AppContext.Provider value={{ routes }}>
            <Provider store={store}>
                <PersistGate loading={null} persistor={persistor}>
                    <Router history={history}>
                        <TsinSuspense>
                            <Layout />
                        </TsinSuspense>
                    </Router>
                </PersistGate>
            </Provider>
        </AppContext.Provider>
    );
};
export default App;