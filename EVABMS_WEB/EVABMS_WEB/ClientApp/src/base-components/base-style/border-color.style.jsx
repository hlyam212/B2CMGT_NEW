import tw from "twin.macro";

const BorderColorVariants = {
    transparent: tw`border-transparent`,
    current: tw`border-current`,
    white: tw`border-white`,
    gray: tw`border-gray-400`,
    gray600: tw`border-gray-600`,
    blue: tw`border-blue-400`,
    blue600: tw`border-blue-600`,
    red: tw`border-red-400`,
    green: tw`border-green-400`,
};
export default BorderColorVariants;

export const HoverBorderColorVariant = {
    white: tw`hover:border-white`,
    gray: tw`hover:border-gray-500`,
    gray200: tw`hover:border-gray-200`,
    blue: tw`hover:border-blue-500`,
};

export const FocusBorderColorVariant = {
    gray: tw`focus-within:(border-gray-600 border-opacity-70)`,
    blue: tw`focus-within:(border-blue-600 border-opacity-70)`,
    red: tw`focus-within:(border-red-600 border-opacity-70)`,
    green: tw`focus-within:(border-green-600 border-opacity-70)`,
};
