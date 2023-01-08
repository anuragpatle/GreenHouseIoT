import React from 'react';

const dashboardRoutes = [
    {
        path: '/dashboard',
        component: React.lazy(() => import('./Dashboard'))
    }
];

export default dashboardRoutes;