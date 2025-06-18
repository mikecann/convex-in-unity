// NOTE: You can remove this file. Declaring the shape
// of the database is entirely optional in Convex.
// See https://docs.convex.dev/database/schemas.

import { defineSchema, defineTable } from "convex/server";
import { v } from "convex/values";

export const fruitKinds = [
  v.literal("banana"),
  v.literal("watermelon"),
  v.literal("strawberry"),
  v.literal("orange"),
  v.literal("apple"),
];

export default defineSchema({
  messages: defineTable({
    message: v.string(),
    user: v.string(),
  }),
  fruit: defineTable({
    kind: v.union(...fruitKinds),
  }),
  fruitGame: defineTable({
    numFruitPopped: v.number(),
  }),
});
