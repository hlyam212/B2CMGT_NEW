import { Home } from "./page/Home/Home";
import ParameterSetting from "./page/ParameterSetting/ParameterSetting";
import Authorization from "./page/Authorization/Authorization";
import Lottery from "./page/Lottery/Lottery";
import Query from "./page/Connecting String/Query";
import FileManagement from "./page/Connecting String/FileManagement";
import RoleQuery from "./page/UserRole/RoleQuery";
import RoleModify from "./page/UserRole/RoleModify";
import SurveyLottery from "./page/Survey/Lottery/Lottery";

const AppRoutes = [
    {
        index: true,
        path: "/Home",
        element: <Home />,
        nav: "Home",
    },
    {
        path: "/ParameterSetting",
        element: <ParameterSetting />,
        nav: "Parameter Setting",
    },
    {
        path: "/Authorization",
        element: <Authorization />,
        nav: "Authorization",
    },
    {
        path: "/Lottery",
        element: <Lottery />,
        nav: "Lottery",
    },
    {
        path: "/Query",
        element: <Query />,
        nav: "Query",
    },
    {
        path: "/FileManagement",
        element: <FileManagement />,
        nav: "File Management",
    },
    {
        path: "/RoleQuery",
        element: <RoleQuery />,
        nav: "RoleQuery",
    },
    {
        path: "/RoleModify",
        element: <RoleModify />,
        nav: "RoleModify",
    },
    {
        path: "/Survey/Lottery",
        element: <SurveyLottery />,
        nav: "Survey",
    },
];

export default AppRoutes;
