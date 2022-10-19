import React, { useState } from 'react';
import { Collapse, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import './NavMenu.css';


const NavMenu = (props) => {
    const [collapsed, setCollapsed] = useState(true);

    const toggleNavbar = () => {
        setCollapsed(!collapsed);
    }

    return (
      <header>
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
        </Navbar>
      </header>
    );
}

export default NavMenu;