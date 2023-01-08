import React from 'react';
import { Link } from 'react-router-dom';
import tsinLogo from '../../../assets/images/tsin-logo.svg';
import { Dropdown } from 'react-bootstrap';
import userImageDummy from '../../../assets/images/user.png';

const Navbar = () => {
    const toggleOffcanvas = () => {
        document.querySelector('.sidebar-offcanvas').classList.toggle('active');
    };
    return (
        <nav className="navbar default-layout-navbar col-lg-12 col-12 p-0 fixed-top d-flex flex-row">
            <div className="text-center navbar-brand-wrapper d-flex align-items-center justify-content-center">
                <Link className="navbar-brand brand-logo-mini" to="/"><img src={tsinLogo} /></Link>
            </div>
            <div className="navbar-menu-wrapper d-flex align-items-stretch">
                <ul className="navbar-nav navbar-nav-right">
                    <li className="nav-item nav-profile">
                        <Dropdown>
                            <Dropdown.Toggle className="nav-link">
                                <div className="nav-profile-img">
                                    <img src={userImageDummy} alt="user" />
                                    <span className="availability-status online"></span>
                                </div>
                                <div className="nav-profile-text">
                                    <p className="mb-1 text-white"><span>Administrator</span></p>
                                </div>
                            </Dropdown.Toggle>

                            <Dropdown.Menu className="navbar-dropdown">
                                <Dropdown.Item href="!#" onClick={evt => {
                                    evt.preventDefault(); alert("Logout called.");
                                }}>
                                    <i className="mdi mdi-logout mr-2 text-primary"></i>
                                    <span style={{ color: 'black' }}>Signout</span>
                                </Dropdown.Item>
                            </Dropdown.Menu>
                        </Dropdown>
                    </li>
                </ul>
                <button className="navbar-toggler navbar-toggler-right d-lg-none align-self-center" type="button" style={{ color: 'white' }} onClick={() => toggleOffcanvas}>
                    <span className="mdi mdi-menu"></span>
                </button>
            </div>
        </nav>
    );
};

export default Navbar;