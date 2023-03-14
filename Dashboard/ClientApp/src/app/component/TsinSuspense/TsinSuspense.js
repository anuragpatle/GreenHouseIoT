import React, { Suspense } from 'react';
import TsinLoader from '../TsinLoading/TsinLoader';

const TsinSuspense = ({ children }) => {
    return <Suspense fallback={<TsinLoader />}>{children}</Suspense>
};

export default TsinSuspense;
