import React from 'react';
import { Navigate } from 'react-router-dom';

import dashboardRoutes from '../../pages/dashboard/DashboardRoute';

const redirectRoute = [
    {
        path: '/',
        exact: true,
        component: React.lazy(() => <Navigate to="/dashboard" replace={true} />)
    }
];

const routes = [
    ...redirectRoute,
    ...dashboardRoutes,
];

export default routes;