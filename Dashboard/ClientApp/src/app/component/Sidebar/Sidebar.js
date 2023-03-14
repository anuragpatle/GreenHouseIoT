import React from 'react';
import { Link, useLocation } from 'react-router-dom';

const Sidebar = () => {
    const location = useLocation();

    React.useEffect(() => {
        const body = document.querySelector('body');
        document.querySelectorAll('.sidebar .nav-item').forEach((el) => {

            el.addEventListener('mouseover', function () {
                if (body.classList.contains('sidebar-icon-only')) {
                    el.classList.add('hover-open');
                }
            });
            el.addEventListener('mouseout', function () {
                if (body.classList.contains('sidebar-icon-only')) {
                    el.classList.remove('hover-open');
                }
            });
        });
    }, []);

    const isPathActive = (loc) => {
        if (location.pathname === loc) {
            return true;
        }
        return false;
    };

    return (
        <nav className="sidebar sidebar-offcanvas" id="sidebar">
            <ul className="nav">
                <li className={isPathActive('/') || isPathActive('/dashboard') ? 'nav-item active' : 'nav-item'}>
                    <Link className="nav-link" to="/dashboard">
                        <span className="menu-title">Dashboard</span>
                        <i className="mdi mdi-home menu-icon"></i>
                    </Link>
                </li>
                <li className={isPathActive('/sustainability') ? 'nav-item active' : 'nav-item'}>
                    <Link className="nav-link" to="/sustainability">
                        <span className="menu-title">Sustainbility</span>
                        <i className="mdi mdi-format-list-bulleted menu-icon"></i>
                    </Link>
                </li>
            </ul>
        </nav>
    );
};

export default Sidebar;