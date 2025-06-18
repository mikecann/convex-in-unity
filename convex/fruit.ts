import { v } from "convex/values";
import { query, mutation, internalMutation } from "./_generated/server";
import { fruitKinds } from "./schema";
import { internal } from "./_generated/api";

export const list = query({
  args: {},
  handler: async (ctx, args) => {
    return await ctx.db.query("fruit").collect();
  },
});

export const pop = mutation({
  args: {
    fruitId: v.id("fruit"),
  },
  handler: async (ctx, args) => {
    await ctx.db.delete(args.fruitId);
    await ctx.scheduler.runAfter(
      Math.round(Math.random() * 1000),
      internal.fruit.spawn
    );
  },
});

export const spawn = internalMutation({
  args: {},
  handler: async (ctx, args) => {
    await ctx.db.insert("fruit", {
      kind: fruitKinds[Math.floor(Math.random() * fruitKinds.length)].value,
      initialPosition: {
        x: Math.random() * 20 - 10, // Random x between -10 and 10
        y: Math.random() * 5 + 5, // Random y between 5 and 10 (spawn above ground)
        z: Math.random() * 20 - 10, // Random z between -10 and 10
      },
      initialVelocity: {
        x: Math.random() * 10 - 5, // Random x velocity between -5 and 5
        y: Math.random() * 5 + 2, // Random y velocity between 2 and 7 (upward)
        z: Math.random() * 10 - 5, // Random z velocity between -5 and 5
      },
    });
  },
});

export const getGame = query({
  args: {},
  handler: async (ctx, args) => {
    return await ctx.db.query("fruitGame").first();
  },
});

export const ensureGameIsCreated = mutation({
  args: {},
  handler: async (ctx, args) => {
    const game = await ctx.db.query("fruitGame").first();
    if (game) return;

    // Create the game and return some initial fruit
    await ctx.db.insert("fruitGame", {
      numFruitPopped: 0,
    });

    // Spawn some initial fruit
    for (let i = 0; i < 10; i++) {
      await ctx.scheduler.runAfter(
        Math.round(Math.random() * 1000),
        internal.fruit.spawn
      );
    }
  },
});
