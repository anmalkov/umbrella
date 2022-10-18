import Entities from "./components/Entities";
import Areas from "./components/Areas";
import Groups from "./components/Groups";
import Extensions from "./components/Extensions";
import Dashboard from "./components/Dashboard";

const AppRoutes = [
  {
    index: true,
    element: <Dashboard />
  },
  {
    path: '/entities',
    element: <Entities />
  },
  {
    path: '/extensions',
    element: <Extensions />
  },
  {
    path: '/areas',
    element: <Areas />
  },
  {
    path: '/groups',
    element: <Groups />
  }
];

export default AppRoutes;
