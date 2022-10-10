import Entities from "./components/Entities";
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
  }
];

export default AppRoutes;
