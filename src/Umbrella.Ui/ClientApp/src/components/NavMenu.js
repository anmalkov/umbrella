import React, { useEffect, useState } from 'react';
import { Collapse, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import { GoChevronDown } from "react-icons/go";
import './NavMenu.css';


const NavMenu = (props) => {
    const [collapsed, setCollapsed] = useState(true);
    const [isMinimal, setIsMinimal] = useState(false);
    const [timer, setTimer] = useState(null);

    const toggleNavbar = () => {
        setCollapsed(!collapsed);
    }

    const toggleIsMinimal = () => {
        setIsMinimal(!isMinimal);
    }

    useEffect(() => {
        if (isMinimal) {
            clearTimeout(timer);
            setTimer(null);
            return;
        }
        setTimer(setTimeout(() => {
            toggleIsMinimal();
        }, 10000));
    }, [isMinimal]);

    return (
        <header>
            {isMinimal ? (
                <div className="position-relative bg-dark navbar-minimal mb-3" onClick={toggleIsMinimal}><GoChevronDown className="position-absolute start-50" /></div>
            ): (
                <Navbar className="navbar-expand-sm navbar-toggleable-sm navbar-dark bg-dark box-shadow mb-3" dark>
                    <NavbarBrand tag={Link} to="/"><b>U</b></NavbarBrand>
                    <NavbarToggler onClick={toggleNavbar} className="mr-2" />
                    <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!collapsed} navbar>
                        <ul className="navbar-nav flex-grow">
                            <NavItem>
                                <NavLink tag={Link} className="text-light" to="/">Dashboard</NavLink>
                            </NavItem>
                            <NavItem>
                                <NavLink tag={Link} className="text-light" to="/entities">Entities</NavLink>
                            </NavItem>
                            <NavItem>
                                <NavLink tag={Link} className="text-light" to="/areas">Areas</NavLink>
                            </NavItem>
                            <NavItem>
                                <NavLink tag={Link} className="text-light" to="/groups">Groups</NavLink>
                            </NavItem>
                            <NavItem>
                                <NavLink tag={Link} className="text-light" to="/extensions">Extensions</NavLink>
                            </NavItem>
                        </ul>
                    </Collapse>
                </Navbar >
            )}
        </header>
    );
}

export default NavMenu;