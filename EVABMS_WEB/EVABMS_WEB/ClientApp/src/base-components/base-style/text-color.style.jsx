import tw from "twin.macro";

const TextColorVariants = {
    transparent: tw`text-transparent`,
    white: tw`text-white`,
    gray: tw`text-gray-700`,
    gray400: tw`text-gray-400`,
    gray500: tw`text-gray-500`,
    gray600: tw`text-gray-600`,
    gray700: tw`text-gray-700`,
    gray900: tw`text-gray-900`,
    blue: tw`text-blue-700`,
    blue200: tw`text-blue-200`,
    blue400: tw`text-blue-400`,
    blue600: tw`text-blue-600`,
    red600: tw`text-red-600`,
    green600: tw`text-green-600`,
    yellow600: tw`text-yellow-600`,
};
export default TextColorVariants;

export const HoverTextColorVariant = {
    white: tw`hover:text-white`,
    gray: tw`hover:text-gray-500`,
    gray200: tw`hover:text-gray-200`,
    blue: tw`hover:text-blue-500`,
};
