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
