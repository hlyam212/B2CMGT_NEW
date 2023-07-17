import tw, { css } from "twin.macro";

const BgColorVariants = {
    blur: tw`backdrop-filter backdrop-blur`,
    transparent: tw`bg-transparent`,
    none: css`
        background: none;
    `,
    white: tw`bg-white`,
    gray: tw`bg-gray-600`,
    gray50: tw`bg-gray-50`,
    gray100: tw`bg-gray-100`,
    blue: tw`bg-blue-600`,
    blue50: tw`bg-blue-50`,
    blue100: tw`bg-blue-100`,
    blue600: tw`bg-blue-600`,
    red100: tw`bg-red-100`,
    red600: tw`bg-red-600`,
    green100: tw`bg-green-100`,
    green600: tw`bg-green-600`,
    yellow100: tw`bg-yellow-100`,
    yellow600: tw`bg-yellow-600`,
};
export default BgColorVariants;

export const HoverBgColorVariant = {
    white: tw`hover:bg-white`,
    gray: tw`hover:bg-gray-600`,
    gray50: tw`hover:bg-gray-50`,
    gray200: tw`hover:bg-gray-200`,
    blue: tw`hover:bg-blue-600`,
    blue50: tw`hover:bg-blue-50`,
    blue200: tw`hover:bg-blue-200`,
    blue700: tw`hover:bg-blue-700`,
    red700: tw`hover:bg-red-700`,
    green700: tw`hover:bg-green-700`,
    yellow700: tw`hover:bg-yellow-700`,
};
