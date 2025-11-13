-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 16-06-2025 a las 23:29:51
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `pokemarket`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `cards`
--

CREATE TABLE `cards` (
  `Id` int(11) NOT NULL,
  `UserID` int(11) NOT NULL,
  `PokemonName` varchar(50) NOT NULL,
  `PokemonType` longtext NOT NULL,
  `ImageUrl` longtext NOT NULL,
  `Collection` longtext NOT NULL,
  `Graded` tinyint(1) NOT NULL,
  `Grade` int(11) NOT NULL,
  `Price` decimal(10,2) DEFAULT NULL,
  `Is_tradeable` tinyint(1) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `LastUpdatedAt` datetime(6) NOT NULL,
  `Rarity` longtext NOT NULL,
  `GradeRequested` tinyint(1) NOT NULL DEFAULT 0,
  `GradeStatus` varchar(20) NOT NULL DEFAULT ''
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `cards`
--

INSERT INTO `cards` (`Id`, `UserID`, `PokemonName`, `PokemonType`, `ImageUrl`, `Collection`, `Graded`, `Grade`, `Price`, `Is_tradeable`, `CreatedAt`, `LastUpdatedAt`, `Rarity`, `GradeRequested`, `GradeStatus`) VALUES
(10, 3, 'Pikachu', 'Eléctrico', 'https://i.ibb.co/kVfVk7bF/pikachu.png', '151', 1, 3, 100.00, 0, '2025-06-11 13:01:22.412721', '2025-06-16 03:05:31.474873', 'Común', 1, 'accepted'),
(12, 3, 'Abra', 'Psíquico', 'http://localhost:5109/images/61f335a1-436a-44ca-b2ad-9d56269df802.png', '151', 0, 0, 12.00, 0, '2025-06-11 13:34:28.982161', '2025-06-16 03:29:39.358761', 'comun', 1, 'requested'),
(13, 3, 'Abra', 'Psíquico', 'http://localhost:5109/images/61f335a1-436a-44ca-b2ad-9d56269df802.png', '151', 0, 0, 12.00, 0, '2025-06-11 13:34:28.982161', '2025-06-16 03:29:39.358761', 'comun', 1, 'requested'),
(14, 3, 'Abra', 'Psíquico', 'http://localhost:5109/images/61f335a1-436a-44ca-b2ad-9d56269df802.png', '151', 0, 0, 12.00, 0, '2025-06-11 13:34:28.982161', '2025-06-16 03:29:39.358761', 'comun', 1, 'requested'),
(15, 3, 'Abra', 'Psíquico', 'http://localhost:5109/images/61f335a1-436a-44ca-b2ad-9d56269df802.png', '151', 0, 0, 12.00, 0, '2025-06-11 13:34:28.982161', '2025-06-16 03:29:39.358761', 'comun', 1, 'requested'),
(16, 3, 'Abra', 'Psíquico', 'http://localhost:5109/images/61f335a1-436a-44ca-b2ad-9d56269df802.png', '151', 0, 0, 12.00, 0, '2025-06-11 13:34:28.982161', '2025-06-16 03:29:39.358761', 'comun', 1, 'requested'),
(17, 3, 'Abra', 'Psíquico', 'http://localhost:5109/images/61f335a1-436a-44ca-b2ad-9d56269df802.png', '151', 0, 0, 12.00, 0, '2025-06-11 13:34:28.982161', '2025-06-16 03:29:39.358761', 'comun', 1, 'requested'),
(18, 3, 'Abra', 'Psíquico', 'http://localhost:5109/images/61f335a1-436a-44ca-b2ad-9d56269df802.png', '151', 0, 0, 12.00, 0, '2025-06-11 13:34:28.982161', '2025-06-16 03:29:39.358761', 'comun', 1, 'requested'),
(19, 3, 'Pikachu', 'Eléctrico', 'https://i.ibb.co/kVfVk7bF/pikachu.png', '151', 1, 3, 100.00, 0, '2025-06-11 13:01:22.412721', '2025-06-16 03:05:31.474873', 'Común', 1, 'accepted'),
(20, 3, 'Pikachu', 'Eléctrico', 'https://i.ibb.co/kVfVk7bF/pikachu.png', '151', 1, 3, 100.00, 0, '2025-06-11 13:01:22.412721', '2025-06-16 03:05:31.474873', 'Común', 1, 'accepted'),
(21, 3, 'Pikachu', 'Eléctrico', 'https://i.ibb.co/kVfVk7bF/pikachu.png', '151', 1, 3, 100.00, 0, '2025-06-11 13:01:22.412721', '2025-06-16 03:05:31.474873', 'Común', 1, 'accepted');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `cardtags`
--

CREATE TABLE `cardtags` (
  `CardId` int(11) NOT NULL,
  `TagId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `cartitems`
--

CREATE TABLE `cartitems` (
  `Id` int(11) NOT NULL,
  `UserId` int(11) NOT NULL,
  `CardId` int(11) NOT NULL,
  `Quantity` int(11) NOT NULL,
  `AddedAt` datetime(6) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `favorites`
--

CREATE TABLE `favorites` (
  `Id` int(11) NOT NULL,
  `UserId` int(11) NOT NULL,
  `CardId` int(11) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `favorites`
--

INSERT INTO `favorites` (`Id`, `UserId`, `CardId`, `CreatedAt`) VALUES
(26, 2, 12, '2025-06-16 03:42:46.983438');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `gradings`
--

CREATE TABLE `gradings` (
  `Id` int(11) NOT NULL,
  `CardId` int(11) NOT NULL,
  `Grade` int(11) NOT NULL,
  `GradedById` int(11) NOT NULL,
  `GradedAt` datetime(6) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `gradings`
--

INSERT INTO `gradings` (`Id`, `CardId`, `Grade`, `GradedById`, `GradedAt`) VALUES
(3, 10, 3, 2, '2025-06-16 03:05:31.460928');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tags`
--

CREATE TABLE `tags` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `LastUpdatedAt` datetime(6) NOT NULL DEFAULT '0001-01-01 00:00:00.000000'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `tags`
--

INSERT INTO `tags` (`Id`, `Name`, `CreatedAt`, `LastUpdatedAt`) VALUES
(1, 'Holográfico', '2025-04-19 13:22:56.740437', '2025-04-19 13:25:58.201174'),
(3, 'Reverse', '2025-06-11 18:42:56.439047', '2025-06-11 18:42:56.421802');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `trades`
--

CREATE TABLE `trades` (
  `Id` int(11) NOT NULL,
  `RequesterId` int(11) NOT NULL,
  `ReceiverId` int(11) NOT NULL,
  `OfferedCardId` int(11) NOT NULL,
  `RequestedCardId` int(11) NOT NULL,
  `Status` int(11) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `LastUpdatedAt` datetime(6) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `users`
--

CREATE TABLE `users` (
  `Id` int(11) NOT NULL,
  `Username` varchar(30) NOT NULL,
  `FirstName` varchar(30) NOT NULL,
  `LastName1` varchar(30) NOT NULL,
  `LastName2` varchar(30) DEFAULT NULL,
  `EmailAddress` longtext NOT NULL,
  `PasswordHash` longtext NOT NULL,
  `PhoneNumber` longtext DEFAULT NULL,
  `Address` longtext DEFAULT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `LastUpdatedAt` datetime(6) NOT NULL,
  `Role` longtext NOT NULL,
  `EmailConfirmationToken` longtext DEFAULT NULL,
  `EmailConfirmationTokenExpiresAt` datetime(6) DEFAULT NULL,
  `IsEmailConfirmed` tinyint(1) NOT NULL DEFAULT 0,
  `PasswordResetToken` longtext DEFAULT NULL,
  `PasswordResetTokenExpiresAt` datetime(6) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `users`
--

INSERT INTO `users` (`Id`, `Username`, `FirstName`, `LastName1`, `LastName2`, `EmailAddress`, `PasswordHash`, `PhoneNumber`, `Address`, `CreatedAt`, `LastUpdatedAt`, `Role`, `EmailConfirmationToken`, `EmailConfirmationTokenExpiresAt`, `IsEmailConfirmed`, `PasswordResetToken`, `PasswordResetTokenExpiresAt`) VALUES
(2, 'erisita', '', '', NULL, 'erisota@AdminPokeMarket.com', 'AQAAAAIAAYagAAAAEL6ikR8xbVGssP61FH4C8p7LmQczmnyJv5udYDpjZQlaaZN3FTnkBJNfG6TlL6S5lQ==', NULL, NULL, '2025-06-11 16:40:36.163084', '2025-06-11 21:20:49.738604', 'admin', '4vitFhjDKF7+T47A46jsuGEwUYh4msfT3im+3rDXOzA=', '2025-06-12 16:40:36.170747', 1, NULL, NULL),
(3, 'erikaka', 'Erika', 'Toledano', 'Morgádez', 'erikmorg05@gmail.com', 'AQAAAAIAAYagAAAAEO1xUS5ESGYJZbORMhje9X1BxyKOmD4UT18iCDWIvOiiY5ezYxa268WQNrebJL+7+w==', '622921208', 'calle Paseo de Móstoles, 143, Parque Coimbra, Móstoles, Madrid', '2025-05-28 15:50:08.597961', '2025-06-02 11:30:29.801776', 'user', NULL, NULL, 1, 'p1NzqF7T6coEBxaPl16fQsyVhgPMMgGqAqaXY8Dcqbw=', '2025-06-02 12:30:29.801775'),
(4, 'erisita', '', '', NULL, 'erikatoledanoinsti@gmail.com', 'AQAAAAIAAYagAAAAEGNdJlWOU4xXGfpeT0CcJq1zmSpD3Hu6cqJJ0Tm0Kj7MkbTrcHL8y3ad3UehXQkQrg==', NULL, NULL, '2025-06-01 18:57:45.953008', '2025-06-02 16:01:19.895368', 'user', NULL, NULL, 1, NULL, NULL),
(5, 'erikmorg05@yahoo.es', '', '', NULL, 'erikmorg05@yahoo.es', 'AQAAAAIAAYagAAAAEOEfyYleTD6mJ6bOVeIpZHRZATVn6kcVgIKH5AGeoiWof4sqIZEi4g30a6DBpDQi+Q==', NULL, NULL, '2025-06-02 12:18:44.108122', '2025-06-02 12:18:44.108136', 'user', 'qbh8/Xpeq0dk8BNPwuDsOLqGWawO92mPz+2XWVYkzjI=', '2025-06-03 12:18:44.111439', 0, NULL, NULL),
(6, 'jovisito', '', '', NULL, 'jovydas09@gmail.com', 'AQAAAAIAAYagAAAAEEXSEbW1j+GQmA5zYU0vxAACBFko35FmkmmAJ5BsqxvOlWJiO+u7YPpEE/u6UIr8/Q==', NULL, NULL, '2025-06-02 18:43:58.253766', '2025-06-02 18:43:58.253780', 'user', 'qPBJxbrnR5LHJnY3QKhugc2vaBmXZcH858jpYMpc+/0=', '2025-06-03 18:43:58.254431', 0, NULL, NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `__efmigrationshistory`
--

CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `__efmigrationshistory`
--

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`) VALUES
('20250403161512_CreateCardsTable', '8.0.13'),
('20250407114800_CreateUsersTable', '8.0.13'),
('20250408135211_CreateFavoritesTable', '8.0.13'),
('20250409141757_AddUserRoleColumn', '8.0.13'),
('20250415124331_AddTagsAndCardTags', '8.0.13'),
('20250415124726_UpdateModels', '8.0.13'),
('20250417100723_AddLastUpdatedAtToTag', '8.0.13'),
('20250419120315_AddLastUpdatedCardModel', '8.0.13'),
('20250419120514_AddLastUpdatedCardModel2', '8.0.13'),
('20250419120718_AddLastUpdatedCardModel3', '8.0.13'),
('20250419121047_AddLastUpdatedCardModel4', '8.0.13'),
('20250419121614_AddLastUpdatedCardModel5', '8.0.13'),
('20250520113421_FinalProjectMigration', '8.0.13'),
('20250522150157_AddCartItemModel', '8.0.13'),
('20250527093348_updatedCartModel', '8.0.13'),
('20250601165050_updatedregisterRequestDTO', '8.0.13'),
('20250601170133_updatedregisterRequestDTO2', '8.0.13');

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `cards`
--
ALTER TABLE `cards`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_Cards_UserID` (`UserID`);

--
-- Indices de la tabla `cardtags`
--
ALTER TABLE `cardtags`
  ADD PRIMARY KEY (`CardId`,`TagId`),
  ADD KEY `IX_CardTags_TagId` (`TagId`);

--
-- Indices de la tabla `cartitems`
--
ALTER TABLE `cartitems`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_CartItems_CardId` (`CardId`),
  ADD KEY `IX_CartItems_UserId` (`UserId`);

--
-- Indices de la tabla `favorites`
--
ALTER TABLE `favorites`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_Favorites_CardId` (`CardId`);

--
-- Indices de la tabla `gradings`
--
ALTER TABLE `gradings`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_Gradings_CardId` (`CardId`),
  ADD KEY `IX_Gradings_GradedById` (`GradedById`);

--
-- Indices de la tabla `tags`
--
ALTER TABLE `tags`
  ADD PRIMARY KEY (`Id`);

--
-- Indices de la tabla `trades`
--
ALTER TABLE `trades`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_Trades_OfferedCardId` (`OfferedCardId`),
  ADD KEY `IX_Trades_ReceiverId` (`ReceiverId`),
  ADD KEY `IX_Trades_RequestedCardId` (`RequestedCardId`),
  ADD KEY `IX_Trades_RequesterId` (`RequesterId`);

--
-- Indices de la tabla `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`Id`);

--
-- Indices de la tabla `__efmigrationshistory`
--
ALTER TABLE `__efmigrationshistory`
  ADD PRIMARY KEY (`MigrationId`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `cards`
--
ALTER TABLE `cards`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=22;

--
-- AUTO_INCREMENT de la tabla `cartitems`
--
ALTER TABLE `cartitems`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=54;

--
-- AUTO_INCREMENT de la tabla `favorites`
--
ALTER TABLE `favorites`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=27;

--
-- AUTO_INCREMENT de la tabla `gradings`
--
ALTER TABLE `gradings`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de la tabla `tags`
--
ALTER TABLE `tags`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de la tabla `trades`
--
ALTER TABLE `trades`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `users`
--
ALTER TABLE `users`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `cards`
--
ALTER TABLE `cards`
  ADD CONSTRAINT `FK_Cards_Users_UserID` FOREIGN KEY (`UserID`) REFERENCES `users` (`Id`) ON DELETE CASCADE;

--
-- Filtros para la tabla `cardtags`
--
ALTER TABLE `cardtags`
  ADD CONSTRAINT `FK_CardTags_Cards_CardId` FOREIGN KEY (`CardId`) REFERENCES `cards` (`Id`) ON DELETE CASCADE,
  ADD CONSTRAINT `FK_CardTags_Tags_TagId` FOREIGN KEY (`TagId`) REFERENCES `tags` (`Id`) ON DELETE CASCADE;

--
-- Filtros para la tabla `cartitems`
--
ALTER TABLE `cartitems`
  ADD CONSTRAINT `FK_CartItems_Cards_CardId` FOREIGN KEY (`CardId`) REFERENCES `cards` (`Id`) ON DELETE CASCADE,
  ADD CONSTRAINT `FK_CartItems_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE;

--
-- Filtros para la tabla `favorites`
--
ALTER TABLE `favorites`
  ADD CONSTRAINT `FK_Favorites_Cards_CardId` FOREIGN KEY (`CardId`) REFERENCES `cards` (`Id`) ON DELETE CASCADE;

--
-- Filtros para la tabla `gradings`
--
ALTER TABLE `gradings`
  ADD CONSTRAINT `FK_Gradings_Cards_CardId` FOREIGN KEY (`CardId`) REFERENCES `cards` (`Id`) ON DELETE CASCADE,
  ADD CONSTRAINT `FK_Gradings_Users_GradedById` FOREIGN KEY (`GradedById`) REFERENCES `users` (`Id`) ON DELETE CASCADE;

--
-- Filtros para la tabla `trades`
--
ALTER TABLE `trades`
  ADD CONSTRAINT `FK_Trades_Cards_OfferedCardId` FOREIGN KEY (`OfferedCardId`) REFERENCES `cards` (`Id`),
  ADD CONSTRAINT `FK_Trades_Cards_RequestedCardId` FOREIGN KEY (`RequestedCardId`) REFERENCES `cards` (`Id`),
  ADD CONSTRAINT `FK_Trades_Users_ReceiverId` FOREIGN KEY (`ReceiverId`) REFERENCES `users` (`Id`),
  ADD CONSTRAINT `FK_Trades_Users_RequesterId` FOREIGN KEY (`RequesterId`) REFERENCES `users` (`Id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
