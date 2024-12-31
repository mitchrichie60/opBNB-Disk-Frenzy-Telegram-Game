// SPDX-License-Identifier: MIT
pragma solidity ^0.8.17;

// Define the "DiskFrenzyGame" contract
contract DiskFrenzyGame {
    // Mapping to track disk points for each player
    mapping(address => uint256) private diskPoints;

    // Total disk points in the game
    uint256 public totalDiskPoints;

    // Events
    event DiskPointsIncreased(address indexed player, uint256 currentPoints);
    event TotalPointsUpdated(uint256 newTotalPoints);

    // Modifier to ensure valid address
    modifier validAddress(address player) {
        require(player != address(0), "Invalid player address");
        _;
    }

    // Function to increase disk points for a player
    function increaseDiskPoints(
        address player,
        uint256 points
    ) external validAddress(player) {
        require(points > 0, "Points must be greater than zero");

        diskPoints[player] += points;
        totalDiskPoints += points;

        emit DiskPointsIncreased(player, diskPoints[player]);
        emit TotalPointsUpdated(totalDiskPoints);
    }

    // Function to retrieve a player's disk points
    function getDiskPoints(address player) external view returns (uint256) {
        return diskPoints[player];
    }
}
